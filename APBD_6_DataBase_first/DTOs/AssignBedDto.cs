namespace APBD_6_DataBase_first.DTOs;

public class AssignBedDto
{
    public DateTime From { get; set; }

    public DateTime? To { get; set; }

    public string BedType { get; set; } = string.Empty;
    public string Ward { get; set; } = string.Empty;
}