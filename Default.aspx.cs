using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Common.Sql;
using w2.Common.Web;

public partial class _Default : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		if (IsPostBack)
		{
			Session.Clear();
		}
		lErrorMessage.Text = "";
	}

	protected void btnLogin_Click(object sender, EventArgs e)
	{
		var data = (DataRowView)Get(tbLoginId.Text, tbPassword.Text);
		if (data != null)
		{
			if ((string)data["delete_flg"] != "0")
			{
				lErrorMessage.Text = HtmlSanitizer.HtmlEncode((string)data["login_id"] + "は退会済みのアカウントです。");
				tbLoginId.Text = "";
				tbPassword.Text = "";
				return;
			}
		}
		else if (data == null)
		{
			lErrorMessage.Text = HtmlSanitizer.HtmlEncode("ログインIDまたはパスワードには誤りがあります。");
			tbLoginId.Text = "";
			tbPassword.Text = "";
			return;
		}
		var ht = new Hashtable
		{
			{"user_id", (int)data["user_id"]}, // 検索と投稿のためにユーザID保存
			{"name", data["name"].ToString()},
			{"login_id", tbLoginId.Text},
			{"password", tbPassword.Text}
		};
		Session["param"] = ht;
		Response.Redirect("~/Form/Forum/ForumTop.aspx");
	}

	protected DataRowView Get(string login_id, string password)
	{
		using (var accessor = new SqlAccessor()) 
		using (var statement = new SqlStatement("User", "Get"))
		{
			var ht = new Hashtable
			{
				{"login_id", login_id},
				{"password", password}
			};
			var dv = statement.SelectSingleStatementWithOC(accessor, ht);
			return (dv.Count != 0) ? dv[0] : null;
		}
	}

	// ログインIDの最小文字数と最大文字数
	protected void tbLoginId_lengthValidate(object source, ServerValidateEventArgs args)
	{
		try
		{
			string i = args.Value;
			args.IsValid = (i.Length >= 3 && i.Length <= 15);
		}
		catch (Exception)
		{
			args.IsValid = false;
		}
	}
	// パスワードの最小文字数と最大文字数
	protected void tbPassword_lengthValidate(object source, ServerValidateEventArgs args)
	{
		try
		{
			string i = args.Value;
			args.IsValid = (i.Length >= 6 && i.Length <= 15);
		}
		catch (Exception)
		{
			args.IsValid = false;
		}
	}
}