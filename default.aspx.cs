using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI.WebControls;

public partial class Knowhow : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            var sb = this.ConnectionString_Get();
            using(var cn = new SQLiteConnection(sb.ToString()))
            {
                cn.Open();
                using(var cmd = new SQLiteCommand(cn))
                {
                    this.Table_Create(cmd);
                    this.Menu_Set(cmd);
                    if(Session["UPDATE"] != null)
                    {
                        this.Page_Update(cmd);
                    }
                    else if(Session["SEARCH"] != null)
                    {
                        this.Page_Search(cmd);
                    }
                    else
                    {
                        this.Page_Set(cmd);
                    }
                }
            }
            if(Session["LOCK"] != null)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('他のユーザーによって既に変更されているため、操作をキャンセルします。');", true);
                Session.Remove("LOCK");
            }
            BtnSend.Attributes["onclick"] = "this.disabled=true;" + ClientScript.GetPostBackEventReference(BtnSend, null) + ";";
        }
    }
    
    protected void LbtMenu_Click(object sender, CommandEventArgs e)
    {
        Session["CATEGORY"] = e.CommandName;
        Session.Remove("SEARCH");
        Session.Remove("UPDATE");
        Session.Remove("TIMESTAMP");
        Response.Redirect(Request.Url.OriginalString);
    }
    
    protected void LbtUpdate_Click(object sender, CommandEventArgs e)
    {
        Session["UPDATE"] = e.CommandName;
        Session["TIMESTAMP"] = e.CommandArgument.ToString();
        Response.Redirect(Request.Url.OriginalString);
    }
    
    protected void LbtDelete_Click(object sender, CommandEventArgs e)
    {
        if(!Page.IsValid) return;
        var sb = this.ConnectionString_Get();
        using(var cn = new SQLiteConnection(sb.ToString()))
        {
            cn.Open();
            using(var cmd = new SQLiteCommand(cn))
            {
                this.Item_Delete(cmd, this.ToInt(e.CommandName), e.CommandArgument.ToString());
            }
        }
        Response.Redirect(Request.Url.OriginalString);
    }
    
    protected void BtnSearch_Click(object sender, EventArgs e)
    {
        Session["SEARCH"] = TxtSearch.Text;
        Session.Remove("UPDATE");
        Session.Remove("TIMESTAMP");
        Response.Redirect(Request.Url.OriginalString);
    }
    
    protected void BtnSend_Click(object sender, EventArgs e)
    {
        if(!Page.IsValid) return;
        var sb = this.ConnectionString_Get();
        using(var cn = new SQLiteConnection(sb.ToString()))
        {
            cn.Open();
            using(var cmd = new SQLiteCommand(cn))
            {
                if(BtnSend.CommandName == "add")
                {
                    this.Item_Add(cmd, TxtCategory.Text, TxtTitle.Text, TxtAuthor.Text, TxtMessage.Text);
                }
                else if(Session["UPDATE"] != null)
                {
                    this.Item_Update(cmd, TxtCategory.Text, TxtTitle.Text, TxtAuthor.Text, TxtMessage.Text, this.ToInt(this.Session_Get("UPDATE")), this.Session_Get("TIMESTAMP"));
                }
                else
                {
                    Session["LOCK"] = true;
                }
            }
        }
        Session["CATEGORY"] = TxtCategory.Text;
        Session["AUTHOR"] = TxtAuthor.Text;
        Session.Remove("SEARCH");
        Session.Remove("UPDATE");
        Session.Remove("TIMESTAMP");
        Response.Redirect(Request.Url.OriginalString);
    }
    
    protected void BtnCancel_Click(object sender, EventArgs e)
    {
        Session.Remove("UPDATE");
        Session.Remove("TIMESTAMP");
        Response.Redirect(Request.Url.OriginalString);
    }
    
    private SQLiteConnectionStringBuilder ConnectionString_Get()
    {
        return new SQLiteConnectionStringBuilder { DataSource = Server.MapPath("./db.sqlite") };
    }
    
    private void Table_Create(SQLiteCommand cmd)
    {
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS T_ITEMS(" +
                          "  NO        INTEGER   NOT NULL PRIMARY KEY AUTOINCREMENT," +
                          "  CATEGORY  TEXT      NOT NULL," +
                          "  TITLE     TEXT      NOT NULL," +
                          "  AUTHOR    TEXT      NOT NULL," +
                          "  IP        TEXT      NOT NULL," +
                          "  MESSAGE   TEXT      NOT NULL," +
                          "  TS        TIMESTAMP NOT NULL DEFAULT (DATETIME('now', 'localtime'))," +
                          "  DELFLG    INTEGER   NOT NULL DEFAULT 0)";
        cmd.ExecuteNonQuery();
    }
    
    private void Menu_Set(SQLiteCommand cmd)
    {
        cmd.CommandText = "SELECT DISTINCT CATEGORY FROM T_ITEMS WHERE DELFLG = 0 ORDER BY CATEGORY ASC";
        using(var reader = cmd.ExecuteReader())
        {
            var dt = new DataTable();
            dt.Load(reader);
            RepMenu.DataSource = dt;
            RepMenu.DataBind();
        }
    }
    
    private void Page_Set(SQLiteCommand cmd)
    {
        var category = this.Category_Get();
        cmd.CommandText = "SELECT * FROM T_ITEMS WHERE CATEGORY = @CATEGORY AND DELFLG = 0 ORDER BY TS DESC;";
        cmd.Parameters.Add(new SQLiteParameter("@CATEGORY", category));
        using(var reader = cmd.ExecuteReader())
        {
            var dt = new DataTable();
            dt.Load(reader);
            RepMain.DataSource = dt;
            RepMain.DataBind();
        }
        Session["CATEGORY"] = category;
        LblResult.Text = Server.HtmlEncode(category);
        TxtCategory.Text = category;
        TxtTitle.Text = "";
        TxtAuthor.Text = this.Session_Get("AUTHOR");
        TxtMessage.Text = "";
        BtnSend.CommandName = "add";
        BtnCancel.Visible = false;
    }
    
    private void Page_Search(SQLiteCommand cmd)
    {
        var search = this.Session_Get("SEARCH");
        cmd.CommandText = "SELECT * FROM T_ITEMS WHERE (TITLE LIKE '%' || @SEARCH || '%' OR AUTHOR LIKE '%' || @SEARCH || '%' OR MESSAGE LIKE '%' || @SEARCH || '%') AND DELFLG = 0 ORDER BY TS DESC;";
        cmd.Parameters.Add(new SQLiteParameter("@SEARCH", search));
        using(var reader = cmd.ExecuteReader())
        {
            var dt = new DataTable();
            dt.Load(reader);
            RepMain.DataSource = dt;
            RepMain.DataBind();
        }
        LblResult.Text = "検索キーワード「" + Server.HtmlEncode(search) + "」";
        TxtCategory.Text = "";
        TxtTitle.Text = "";
        TxtAuthor.Text = this.Session_Get("AUTHOR");
        TxtMessage.Text = "";
        BtnSend.CommandName = "add";
        BtnCancel.Visible = false;
    }
    
    private void Page_Update(SQLiteCommand cmd)
    {
        var no = this.ToInt(this.Session_Get("UPDATE"));
        cmd.CommandText = "SELECT * FROM T_ITEMS WHERE NO = @NO AND DELFLG = 0;";
        cmd.Parameters.Add(new SQLiteParameter("@NO", no));
        using(var reader = cmd.ExecuteReader())
        {
            if(reader.Read())
            {
                LblResult.Text = "変更";
                TxtCategory.Text = reader["CATEGORY"].ToString();
                TxtTitle.Text = reader["TITLE"].ToString();
                TxtAuthor.Text = reader["AUTHOR"].ToString();
                TxtMessage.Text = reader["MESSAGE"].ToString();
                Session["TIMESTAMP"] = reader["TS"].ToString();
            }
        }
        RepMain.DataSource = null;
        RepMain.DataBind();
        BtnSend.CommandName = "update";
        BtnCancel.Visible = true;
    }
    
    private string Category_Get()
    {
        var dt = (DataTable)RepMenu.DataSource;
        if(Session["CATEGORY"] != null)
        {
            foreach(DataRow row in dt.Rows)
            {
                if(row["CATEGORY"].ToString() == Session["CATEGORY"].ToString())
                {
                    return Session["CATEGORY"].ToString();
                }
            }
        }
        if(dt.Rows.Count > 0)
        {
            return dt.Rows[0]["CATEGORY"].ToString();
        }
        return "";
    }
    
    private string Session_Get(string key)
    {
        if(Session[key] != null)
        {
            return Session[key].ToString();
        }
        return "";
    }
    
    private int ToInt(string val)
    {
        var num = 0;
        int.TryParse(val, out num);
        return num;
    }
    
    private void Item_Delete(SQLiteCommand cmd, int no, string ts)
    {
        cmd.CommandText = "UPDATE T_ITEMS SET TS = DATETIME('now', 'localtime'), DELFLG = 1 WHERE NO = @NO AND TS = @TS";
        cmd.Parameters.Add(new SQLiteParameter("@NO", no));
        cmd.Parameters.Add(new SQLiteParameter("@TS", DateTime.Parse(ts).ToString("yyyy-MM-dd HH:mm:ss")));
        if(cmd.ExecuteNonQuery() < 1)
        {
            Session["LOCK"] = true;
        }
    }
    
    private void Item_Add(SQLiteCommand cmd, string category, string title, string author, string message)
    {
        cmd.CommandText = "INSERT INTO T_ITEMS (CATEGORY, TITLE, AUTHOR, IP, MESSAGE) VALUES (@CATEGORY, @TITLE, @AUTHOR, @IP, @MESSAGE)";
        cmd.Parameters.Add(new SQLiteParameter("@CATEGORY", category));
        cmd.Parameters.Add(new SQLiteParameter("@TITLE", title));
        cmd.Parameters.Add(new SQLiteParameter("@AUTHOR", author));
        cmd.Parameters.Add(new SQLiteParameter("@IP", System.Web.HttpContext.Current.Request.UserHostAddress));
        cmd.Parameters.Add(new SQLiteParameter("@MESSAGE", message));
        if(cmd.ExecuteNonQuery() < 1)
        {
            Session["LOCK"] = true;
        }
    }
    
    private void Item_Update(SQLiteCommand cmd, string category, string title, string author, string message, int no, string ts)
    {
        cmd.CommandText = "UPDATE T_ITEMS SET CATEGORY = @CATEGORY, TITLE = @TITLE, AUTHOR = @AUTHOR, IP = @IP, MESSAGE = @MESSAGE, TS = DATETIME('now', 'localtime') WHERE NO = @NO AND TS = @TS AND DELFLG = 0;";
        cmd.Parameters.Add(new SQLiteParameter("@CATEGORY", category));
        cmd.Parameters.Add(new SQLiteParameter("@TITLE", title));
        cmd.Parameters.Add(new SQLiteParameter("@AUTHOR", author));
        cmd.Parameters.Add(new SQLiteParameter("@IP", System.Web.HttpContext.Current.Request.UserHostAddress));
        cmd.Parameters.Add(new SQLiteParameter("@MESSAGE", message));
        cmd.Parameters.Add(new SQLiteParameter("@NO", no));
        cmd.Parameters.Add(new SQLiteParameter("@TS", DateTime.Parse(ts).ToString("yyyy-MM-dd HH:mm:ss")));
        if(cmd.ExecuteNonQuery() < 1)
        {
            Session["LOCK"] = true;
        }
    }
}
