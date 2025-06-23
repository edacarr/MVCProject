using CmsSample.Models;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CmsSample.Data
{
    public class OracleBlogRepository
    {
        private readonly string _cs;
        public OracleBlogRepository(IConfiguration cfg)
        {
            _cs = cfg.GetConnectionString("OracleDb").ToString();
        }

        // --- LISTE ------------------------------------------------------------
        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            const string q = @"SELECT ID,SLUG,TITLE,EXCERPT,
                                      COVERIMAGEURL,PUBLISHEDON
                               FROM BLOG_POSTS
                               ORDER BY PUBLISHEDON DESC";

            var list = new List<BlogPost>();
            await using var c = new OracleConnection(_cs);
            await c.OpenAsync();
            await using var cmd = new OracleCommand(q, c);
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                list.Add(new BlogPost
                {
                    Id = r.GetInt32(0),
                    Slug = r.GetString(1),
                    Title = r.GetString(2),
                    Excerpt = r.IsDBNull(3) ? null : r.GetString(3),
                    CoverImageUrl = r.IsDBNull(4) ? null : r.GetString(4),
                    PublishedOn = r.GetDateTime(5)
                });
            return list;
        }
        // ───── YORUM EKLE ─────────────────────────────────────────
        public async Task<int> InsertCommentAsync(Comment c)
        {
            const string q = @"INSERT INTO COMMENTS
            (POST_ID, AUTHOR_ID, BODY, POSTED_ON)
            VALUES (:p, :a, :b, SYSTIMESTAMP)";

            await using var conn = new OracleConnection(_cs);
            await conn.OpenAsync();

            await using var cmd = new OracleCommand(q, conn) { BindByName = true };
            cmd.Parameters.Add(":p", c.PostId);
            cmd.Parameters.Add(":a", c.AuthorId);
            cmd.Parameters.Add(":b", c.Body);

            return await cmd.ExecuteNonQueryAsync();   // 1
        }

        // ───── YORUM LİSTELE ──────────────────────────────────────
        public async Task<IEnumerable<Comment>> GetCommentsAsync(int postId)
        {
            const string q = @"SELECT ID, POST_ID, AUTHOR_ID, BODY, POSTED_ON
                       FROM COMMENTS
                       WHERE POST_ID = :p
                       ORDER BY POSTED_ON";

            var list = new List<Comment>();
            await using var conn = new OracleConnection(_cs);
            await conn.OpenAsync();
            await using var cmd = new OracleCommand(q, conn);
            cmd.Parameters.Add(":p", postId);
            await using var r = await cmd.ExecuteReaderAsync();

            while (await r.ReadAsync())
                list.Add(new Comment
                {
                    Id = r.GetInt32(0),
                    PostId = r.GetInt32(1),
                    AuthorId = r.GetString(2),
                    Body = r.GetString(3),
                    PostedOn = r.GetDateTime(4)
                });
            return list;
        }

        // --- TEK KAYIT --------------------------------------------------------
        public async Task<BlogPost?> GetByIdAsync(int id)
        {
            const string q = @"SELECT ID, SLUG, TITLE, EXCERPT, HTMLBODY,
                              COVERIMAGEURL, PUBLISHEDON, AUTHOR_ID   -- ← eklendi
                       FROM BLOG_POSTS
                       WHERE ID = :pId";

            await using var c = new OracleConnection(_cs);
            await c.OpenAsync();

            await using var cmd = new OracleCommand(q, c);
            cmd.Parameters.Add(":pId", id);

            await using var r = await cmd.ExecuteReaderAsync();
            return await r.ReadAsync() ? new BlogPost
            {
                Id = r.GetInt32(0),
                Slug = r.GetString(1),
                Title = r.GetString(2),
                Excerpt = r.IsDBNull(3) ? null : r.GetString(3),
                HtmlBody = r.IsDBNull(4) ? null : r.GetString(4),
                CoverImageUrl = r.IsDBNull(5) ? null : r.GetString(5),
                PublishedOn = r.GetDateTime(6),
                AuthorId = r.GetString(7)                 // ← eklenen satır
            } : null;
        }


        // --- EKLE -------------------------------------------------------------
        public async Task<int> InsertAsync(BlogPost p)
        {
            const string q = @"
    INSERT INTO BLOG_POSTS
      (SLUG, TITLE, EXCERPT, HTMLBODY, COVERIMAGEURL, PUBLISHEDON, AUTHOR_ID)
    VALUES
      (:s,  :t,    :e,      :b,       :c,           SYSTIMESTAMP, :a)";

            await using var c = new OracleConnection(_cs);
            await c.OpenAsync();

            await using var cmd = new OracleCommand(q, c)
            {
                BindByName = true        // sıraya bakmadan ada göre eşler
            };

            cmd.Parameters.Add(":s", p.Slug);
            cmd.Parameters.Add(":t", p.Title);
            cmd.Parameters.Add(":e", p.Excerpt);
            cmd.Parameters.Add(":b", p.HtmlBody);
            cmd.Parameters.Add(":c", p.CoverImageUrl);
            cmd.Parameters.Add(":a", p.AuthorId);   // <-- burada tanımlanır

            return await cmd.ExecuteNonQueryAsync();   // 1 döner
        }


        // --- GÜNCELLE ---------------------------------------------------------
        public async Task<int> UpdateAsync(BlogPost p)
        {
            const string q = @"UPDATE BLOG_POSTS
                               SET SLUG=:s, TITLE=:t, EXCERPT=:e,
                                   HTMLBODY=:b, COVERIMAGEURL=:c
                               WHERE ID=:id";
            await using var c = new OracleConnection(_cs);
            await c.OpenAsync();
            await using var cmd = new OracleCommand(q, c);
            cmd.Parameters.Add(":s", p.Slug);
            cmd.Parameters.Add(":t", p.Title);
            cmd.Parameters.Add(":e", p.Excerpt);
            cmd.Parameters.Add(":b", p.HtmlBody);
            cmd.Parameters.Add(":c", p.CoverImageUrl);
            cmd.Parameters.Add(":id", p.Id);
            return await cmd.ExecuteNonQueryAsync();
        }

        // --- SİL --------------------------------------------------------------
        public async Task<int> DeleteAsync(int id)
        {
            const string q = "DELETE FROM BLOG_POSTS WHERE ID=:pId";
            await using var c = new OracleConnection(_cs);
            await c.OpenAsync();
            await using var cmd = new OracleCommand(q, c);
            cmd.Parameters.Add(":pId", id);
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
