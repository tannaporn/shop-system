using Dapper;

namespace shop_api_layer.Data.SqlBuilder;

/// <summary>
/// Builds a Dapper CommandDefinition from a CommandObject and an optional parameter bag.
/// </summary>
public class DataObject
{
    private readonly CommandObject _command;
    private readonly object? _parameters;

    public DataObject(CommandObject command, object? parameters = null)
    {
        _command = command;
        _parameters = parameters;
    }

    public string GetSelectSql()
    {
        var columns = _command.Columns.Count > 0
            ? string.Join(",\n       ", _command.Columns.OrderBy(c => c.Id).Select(c => c.Name))
            : "*";

        var sql = $"SELECT {columns}\nFROM {_command.TableName}";

        if (!string.IsNullOrWhiteSpace(_command.WhereCondition))
            sql += $"\n{_command.WhereCondition.Trim()}";

        if (!string.IsNullOrWhiteSpace(_command.OrderBy))
            sql += $"\nORDER BY {_command.OrderBy}";

        return sql;
    }

    public CommandDefinition GetCommandDefinition(CancellationToken cancellationToken = default)
        => new CommandDefinition(GetSelectSql(), _parameters, cancellationToken: cancellationToken);
}
