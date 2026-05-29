using APBD_6_DataBase_first.DTOs;

namespace APBD_6_DataBase_first.Services;

public interface IDbService
{
    public Task<IEnumerable<GetPatientsWithAdmissionsDto>> GetPatientsWithAdmissions(string? search);
    public Task AssignBedToPatient(string pesel,AssignBedDto assignDto);
}