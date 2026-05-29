using APBD_6_DataBase_first.Data;
using APBD_6_DataBase_first.DTOs;
using APBD_6_DataBase_first.Exceptions;
using APBD_6_DataBase_first.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD_6_DataBase_first.Services;

public class DbService : IDbService
{
    private readonly ApbdDbFirstContext _context;

    public DbService(ApbdDbFirstContext context)
    {
        _context = context;
    }


    public async Task<IEnumerable<GetPatientsWithAdmissionsDto>> GetPatientsWithAdmissions(string? search)
    {

        var result = await _context.Patients
            .Where(e=>  string.IsNullOrEmpty(search) || e.LastName.Contains(search) ||  e.FirstName.Contains(search))
            .Select(e=> new GetPatientsWithAdmissionsDto()
        {
            Pesel =  e.Pesel,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Age = e.Age,
            Sex = e.Sex,
            Admissions = e.Admissions.Select(a=> new AdmissionWithWardsDto()
            {
                Id = a.Id,
                AdmissionDate = a.AdmissionDate,
                DischargeDate =  a.DischargeDate,
                Ward = new WardDto()
                {
                 Id = a.Ward.Id,
                 Name = a.Ward.Name,
                 Description = a.Ward.Description,
                }
                
            }),
            BedAssignments = e.BedAssignments.Select(b=> new BedAssignmentWithRoomsDto()
            {
                Id = b.Id,
                From =  b.From,
                To =  b.To,
                Bed = new BedDto()
                {
                   Id = b.Bed.Id,
                   BedType = new BedTypeDto()
                   {
                       Id = b.Bed.BedType.Id,
                       Name = b.Bed.BedType.Name,
                       Description =  b.Bed.BedType.Description
                   },
                   Room = new RoomDto()
                   {
                       Id =  b.Bed.Room.Id,
                       HasTv =  b.Bed.Room.HasTv,
                       Ward =  new WardDto()
                       {
                           Id = b.Bed.Room.Ward.Id,
                           Name = b.Bed.Room.Ward.Name,
                           Description =  b.Bed.Room.Ward.Description
                       }
                   }
                }
                
            })
            
            
        }).ToListAsync();


        return result;

    }

    public async Task AssignBedToPatient(string pesel, AssignBedDto assignDto)
    {
        var isPatient = await _context.Patients
            .AnyAsync(a => a.Pesel==pesel);
        if (!isPatient)
        {
            throw new NotFoundException($"No patient with that pesel! {pesel}");
        }
        
        var isWard = await _context.Wards
            .AnyAsync(a => a.Name==assignDto.Ward);
        if (!isWard)
        {
            throw new NotFoundException($"This Ward doesn't exists! {assignDto.Ward}");
        }
        
        var isBedType = await _context.BedTypes
            .AnyAsync(a => a.Name==assignDto.BedType);
        if (!isBedType)
        {
            throw new NotFoundException($"This bed type doesn't exists! {assignDto.BedType}");
        }
        var isBedTypeinThatWard = await _context.Beds
                .AnyAsync(bed => bed.BedType.Name == assignDto.BedType && bed.Room.Ward.Name == assignDto.Ward);
            if (!isBedTypeinThatWard)
            {
                throw new NotFoundException($"No that type of beds in that Ward! bed type => {assignDto.BedType}  ward => {assignDto.Ward}");
            }

        Bed? isAvailable; 
        
        if (assignDto.To.HasValue)
        {
            isAvailable = _context.Beds
                .Where(bed => 
                    bed.Room.Ward.Name == assignDto.Ward 
                    && bed.BedType.Name == assignDto.BedType
                    && bed.BedAssignments.All(assignment =>
                        (assignment.To.HasValue && assignDto.To.Value < assignment.To  && assignDto.From < assignment.From) ||
                        (assignment.To.HasValue && assignDto.To.Value > assignment.To && assignDto.From > assignment.From)||
                        (!assignment.To.HasValue && assignDto.To.Value < assignment.From)
                    )
                
                ).FirstOrDefault();  
        }
        else
        {
            isAvailable = _context.Beds
                .Where(bed => 
                    bed.Room.Ward.Name == assignDto.Ward 
                    && bed.BedType.Name == assignDto.BedType
                    && bed.BedAssignments.All(assignment =>
                        (assignment.To.HasValue && assignment.To.Value < assignment.From)
                    )
                
                ).FirstOrDefault();
            
        }
       
        
        if (isAvailable == null)
        {
            throw new NotFoundException($"No available beds!");
        }
        
        
        var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.BedAssignments.AddAsync(new BedAssignment()
            {
                PatientPesel = pesel,
                BedId = isAvailable.Id,
                From = assignDto.From,
                To = assignDto.To,
            });
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
        

        
   
    }
}