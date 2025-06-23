using CmsSample.Models;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CmsSample.Data   // Data klasöründeyiz
{
    public class OraclePageRepository
    {
        private readonly string _connStr;

        public OraclePageRepository(IConfiguration cfg)
        {
            _connStr = cfg.GetConnectionString("OracleDb");
        }

        public async Task<IEnumerable<StaticPage>> GetAllAsync()
        {
            const string sql = "SELECT ID, SLUG, TITLE, HTMLBODY, LASTUPDATED FROM PAGES";

            var list = new List<StaticPage>();

            await using var conn = new OracleConnection(_connStr);
            await conn.OpenAsync();

            await using var cmd = new OracleCommand(sql, conn);
            await using var rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync())
            {
                list.Add(new StaticPage
                {
                    Id = rdr.GetInt32(0),
                    Slug = rdr.GetString(1),
                    Title = rdr.GetString(2),
                    HtmlBody = rdr.IsDBNull(3) ? null : rdr.GetString(3),
                    LastUpdated = rdr.GetDateTime(4)
                });
            }
            return list;
        }

        public async Task<int> UpdateAsync(StaticPage p)
        {
            const string sql = @"
        UPDATE PAGES
        SET SLUG = :pSlug, TITLE = :pTitle,
            HTMLBODY = :pBody, LASTUPDATED = :pUpd
        WHERE ID = :pId";

            await using var c = new OracleConnection(_connStr);
            await c.OpenAsync();
            await using var cmd = new OracleCommand(sql, c) { BindByName = true };
            cmd.Parameters.Add(":pSlug", p.Slug);
            cmd.Parameters.Add(":pTitle", p.Title);
            cmd.Parameters.Add(":pBody", p.HtmlBody);
            cmd.Parameters.Add(":pUpd", p.LastUpdated);
            cmd.Parameters.Add(":pId", p.Id);
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM PAGES WHERE ID = :pId";
            await using var c = new OracleConnection(_connStr);
            await c.OpenAsync();
            await using var cmd = new OracleCommand(sql, c);
            cmd.Parameters.Add(":pId", id);
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> InsertAsync(StaticPage page)
        {
            const string sql = @"
                INSERT INTO PAGES (SLUG, TITLE, HTMLBODY, LASTUPDATED)
                VALUES (:pSlug, :pTitle, :pBody, :pUpdated)";

            await using var conn = new OracleConnection(_connStr);
            await conn.OpenAsync();

            await using var cmd = new OracleCommand(sql, conn)
            {
                BindByName = true
            };
            cmd.Parameters.Add(":pSlug", page.Slug);
            cmd.Parameters.Add(":pTitle", page.Title);
            cmd.Parameters.Add(":pBody", page.HtmlBody);
            cmd.Parameters.Add(":pUpdated", page.LastUpdated);

            return await cmd.ExecuteNonQueryAsync(); // 1 döner
        }
    }
}
