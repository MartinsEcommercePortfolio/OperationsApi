namespace OperationsApi.Domain.Equipment;

internal sealed class Scanner
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
}