using Dapper;
using IDM.Infrastructure.Database;
using IDM.Infrastructure.Repositories.Interfaces;
using System.Data;

namespace IDM.Infrastructure.Repositories
{
    public abstract class BaseRepository
    {
        private readonly IUserContext _userContext;
        private readonly ConnectionFactory _factory;

        protected BaseRepository(ConnectionFactory factory, IUserContext userContext)
        {
            _factory = factory;
            _userContext = userContext;
        }

        protected IDbConnection CreateConnection() => _factory.Create();

        protected void StampAuditFields(object entity, bool isNew)
        {
            if (entity == null) return;

            var now = DateTime.UtcNow;
            var userId = _userContext.GetUserId();

            var type = entity.GetType();

            if (isNew)
            {
                SetPropertyIfExists(type, entity, "StoreTs", now);
                SetPropertyIfExists(type, entity, "StoredBy", userId);
            }

            SetPropertyIfExists(type, entity, "UpdateTs", now);
            SetPropertyIfExists(type, entity, "UpdateBy", userId);
        }

        private void SetPropertyIfExists(Type type, object entity, string propName, object value)
        {
            var prop = type.GetProperty(propName);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(entity, value);
            }
        }

        protected async Task<int> InsertAsync<T>(string sql, T entity)
        {
            StampAuditFields(entity, isNew: true);

            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql, entity);
        }

        protected async Task<bool> UpdateAsync<T>(string sql, T entity)
        {
            StampAuditFields(entity, isNew: false);

            using var connection = CreateConnection();
            var affected = await connection.ExecuteAsync(sql, entity);
            return affected > 0;
        }

        //for stored proc
        protected async Task<int> InsertAsync<T>(
        string storedProcedure,
        T entity,
        Func<T, DynamicParameters> parameterBuilder)
        {
            StampAuditFields(entity, isNew: true);

            var parameters = parameterBuilder(entity);
            using var connection = CreateConnection();

            return await connection.ExecuteScalarAsync<int>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        protected async Task<bool> UpdateAsync<T>(
            string storedProcedure,
            T entity,
            Func<T, DynamicParameters> parameterBuilder)
        {
            StampAuditFields(entity, isNew: false);

            var parameters = parameterBuilder(entity);
            using var connection = CreateConnection();

            return await connection.ExecuteAsync(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure) > 0;
        }

    }
}
