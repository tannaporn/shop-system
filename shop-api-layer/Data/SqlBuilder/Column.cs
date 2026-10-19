namespace shop_api_layer.Data.SqlBuilder;

public class Column
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsPrimaryKey { get; set; }
    public bool IsRequired { get; set; }
}
