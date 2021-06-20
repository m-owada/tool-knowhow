using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI.WebControls;

public partial class Admin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            BtnSend.Attributes["onclick"] = "this.disabled=true;" + ClientScript.GetPostBackEventReference(BtnSend, null) + ";";
        }
    }
    
    protected void BtnSend_Click(object sender, EventArgs e)
    {
        if(!Page.IsValid) return;
        LblSql.Text = "";
        GrdSql.DataSource = null;
        GrdSql.DataBind();
        var sb = this.ConnectionString_Get();
        using(var cn = new SQLiteConnection(sb.ToString()))
        {
            cn.Open();
            using(var cmd = new SQLiteCommand(cn))
            {
                this.Page_Set(cmd);
            }
        }
    }
    
    private SQLiteConnectionStringBuilder ConnectionString_Get()
    {
        return new SQLiteConnectionStringBuilder { DataSource = Server.MapPath("./db.sqlite") };
    }
    
    private void Page_Set(SQLiteCommand cmd)
    {
        cmd.CommandText = TxtSql.Text;
        try
        {
            using(var reader = cmd.ExecuteReader())
            {
                var dt = new DataTable();
                dt.Load(reader);
                LblSql.Text = "正常 (RowsCount:" + Server.HtmlEncode(dt.Rows.Count.ToString()) + " RecordsAffected:" + Server.HtmlEncode(reader.RecordsAffected.ToString()) + ")";
                GrdSql.DataSource = dt.DefaultView;
                GrdSql.DataBind();
            }
        }
        catch(Exception e)
        {
            LblSql.Text = "エラー (" + Server.HtmlEncode(e.Message) + ")";
        }
    }
}
