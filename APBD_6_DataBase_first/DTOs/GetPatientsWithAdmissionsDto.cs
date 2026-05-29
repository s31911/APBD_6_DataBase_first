using APBD_6_DataBase_first.Models;

namespace APBD_6_DataBase_first.DTOs;

public class GetPatientsWithAdmissionsDto
{
    public string Pesel { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int Age { get; set; }

    public bool Sex { get; set; }

    public IEnumerable<AdmissionWithWardsDto> Admissions { get; set; } = [];

    public IEnumerable<BedAssignmentWithRoomsDto> BedAssignments { get; set; } =[];
}

public class AdmissionWithWardsDto
{
    public int Id { get; set; }

    public DateTime AdmissionDate { get; set; }

    public DateTime? DischargeDate { get; set; }

    public string PatientPesel { get; set; } = null!;
    
    public virtual WardDto Ward { get; set; } = null!;
    
}

public class WardDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
}

public class BedAssignmentWithRoomsDto
{
    public int Id { get; set; }

    public DateTime From { get; set; }

    public DateTime? To { get; set; }

    public BedDto Bed { get ; set; }

}

public class BedDto
{
    public int Id { get; set; }
    public BedTypeDto? BedType { get; set; }
    public RoomDto? Room { get; set; }
}

public class BedTypeDto
{
    public  int Id { get; set; }
    public string Name { get; set; }= string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class RoomDto
{
    public string Id { get; set; } = string.Empty;
    public bool HasTv { get; set; }
    public WardDto? Ward { get; set; }

}