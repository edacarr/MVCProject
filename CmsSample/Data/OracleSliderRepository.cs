using CmsSample.Models;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CmsSample.Data
{
    public class OracleSliderRepository
    {
        private readonly string _cs;
        public OracleSliderRepository(IConfiguration cfg) =>
            _cs = cfg.GetConnectionString("OracleDb");

        public async Task<IEnumerable<SliderItem>> GetAllAsync()
        {
            const string q = "SELECT ID, CAPTION, IMAGEURL, TARGETURL, DISPLAYORDER FROM SLIDER_ITEMS ORDER BY DISPLAYORDER";
            var list = new List<SliderItem>();
            await using var c = new OracleConnection(_cs);
            await c.OpenAsync();
            await using var cmd = new OracleCommand(q, c);
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                list.Add(new SliderItem
                {
                    Id = r.GetInt32(0),
                    Caption = r.GetString(1),
                    ImageUrl = r.GetString(2),
                    TargetUrl = r.GetString(3),
                    DisplayOrder = r.GetInt32(4)
                });
            return list;
        }
        // ----------------- CRUD -----------------
        public async Task<int> InsertAsync(SliderItem s)
        {
            const string sql = @"INSERT INTO SLIDER_ITEMS
                (CAPTION, IMAGEURL, TARGETURL, DISPLAYORDER)
                VALUES (:pCap, :pImg, :pUrl, :pOrd)";
            await using var c = new OracleConnection(_cs);
            await c.OpenAsync();
            await using var cmd = new OracleCommand(sql, c) { BindByName = true };
            cmd.Parameters.Add(":pCap", s.Caption);
            cmd.Parameters.Add(":pImg", s.ImageUrl);
            cmd.Parameters.Add(":pUrl", s.TargetUrl);
            cmd.Parameters.Add(":pOrd", s.DisplayOrder);
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> UpdateAsync(SliderItem s)
        {
            const string sql = @"UPDATE SLIDER_ITEMS SET
                CAPTION=:pCap, IMAGEURL=:pImg, TARGETURL=:pUrl, DISPLAYORDER=:pOrd
                WHERE ID=:pId";
            await using var c = new OracleConnection(_cs);
            await c.OpenAsync();
            await using var cmd = new OracleCommand(sql, c) { BindByName = true };
            cmd.Parameters.Add(":pCap", s.Caption);
            cmd.Parameters.Add(":pImg", s.ImageUrl);
            cmd.Parameters.Add(":pUrl", s.TargetUrl);
            cmd.Parameters.Add(":pOrd", s.DisplayOrder);
            cmd.Parameters.Add(":pId", s.Id);
            return await cmd.ExecuteNonQueryAsync();
        }
        public async Task<SliderItem> GetByIdAsync(int id)
        {
            const string sql = @"
        SELECT ID, CAPTION, IMAGEURL, TARGETURL, DISPLAYORDER
        FROM SLIDER_ITEMS
        WHERE ID = :pId";

            await using var c = new OracleConnection(_cs);
            await c.OpenAsync();
            await using var cmd = new OracleCommand(sql, c);
            cmd.Parameters.Add(":pId", id);

            await using var r = await cmd.ExecuteReaderAsync();
            return await r.ReadAsync()
                ? new SliderItem
                {
                    Id = r.GetInt32(0),
                    Caption = r.GetString(1),
                    ImageUrl = r.GetString(2),
                    TargetUrl = r.GetString(3),
                    DisplayOrder = r.GetInt32(4)
                }
                : null;
        }

        public async Task<int> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM SLIDER_ITEMS WHERE ID = :pId";
            await using var c = new OracleConnection(_cs);
            await c.OpenAsync();
            await using var cmd = new OracleCommand(sql, c);
            cmd.Parameters.Add(":pId", id);
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}