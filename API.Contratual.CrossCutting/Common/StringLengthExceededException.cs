namespace API.Contratual.CrossCutting.Common;

public class StringLengthExceededException : Exception
{
    public StringLengthExceededException(string fieldName, int maxLength, int actualLength) 
        : base($"O campo {fieldName} excede o limite permitido de {maxLength} caracteres. Tamanho atual: {actualLength}")
    {
        FieldName = fieldName;
        MaxLength = maxLength;
        ActualLength = actualLength;
    }

    public string FieldName { get; }
    public int MaxLength { get; }
    public int ActualLength { get; }
}