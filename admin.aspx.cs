using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Web.UI.WebControls;

public partial class Admin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "prompt", "document.getElementById('" + HdnKey.ClientID + "').value=prompt('管理キーを入力してください。','');document.getElementById('" + BtnKey.ClientID + "').click();", true);
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
    
    protected void BtnKey_Click(object sender, EventArgs e)
    {
        if(HdnKey.Value == ConfigurationManager.AppSettings["admin"])
        {
            BtnSend.Enabled = true;
            LblSql.Text = "";
        }
        else
        {
            BtnSend.Enabled = false;
            LblSql.Text = "エラー (管理キーに誤りがあります。)";
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
