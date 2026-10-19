namespace shop_api_layer.Data.SqlBuilder;

public class CommandObject
{
    public string TableName { get; set; } = string.Empty;
    public List<Column> Columns { get; set; } = [];
    public string WhereCondition { get; set; } = string.Empty;
    public string? OrderBy { get; set; }
}
