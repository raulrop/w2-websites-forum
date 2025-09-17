using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Common.Web;
using w2.Common.Sql;

public partial class Form_User_UserModifyInput : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		var ht = (Hashtable)Session["param"];
		// 未ログインのエラー
		if (ht == null)
		{
			Response.Redirect("~/Default.aspx");
			return;
		}
		lErrorMessage.Text = "";
	}
	protected void btnUserModify_Click(object sender, EventArgs e)
	{
		DataRowView drv_login_id = Get_login_id(tbLoginId.Text);
		if (drv_login_id != null)
		{
			lErrorMessage.Text = HtmlSanitizer.HtmlEncode(drv_login_id["login_id"].ToString() + "は既に利用されているログインIDです");
			return;
		}
		var ht = (Hashtable)Session["param"];
		var data = Get(ht["login_id"].ToString(), ht["password"].ToString());
		// ユーザIDの取得をしてからUpdate処理
		var new_ht = new Hashtable
		{
			{"type", "Update"},
			{"user_id", (int)data["user_id"]},
			{"name", tbUserName.Text},
			{"login_id", tbLoginId.Text},
			{"password", tbPassword.Text}
		};
		Session["param"] = new_ht;
		Response.Redirect("~/Form/User/UserRegisterConfirm.aspx");
	}

	protected DataRowView Get_login_id(string login_id)
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("User", "Get_login_id"))
		{
			var ht = new Hashtable
			{
				{"login_id", login_id}
			};
			var dv = statement.SelectSingleStatementWithOC(accessor, ht);
			return (dv.Count != 0) ? dv[0] : null;
		}
	}

	protected DataRowView Get(string login_id, string password)
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("User", "Get"))
		{
			var ht = new Hashtable
			{
				{"login_id", login_id},
				{"password", password},
			};
			var dv = statement.SelectSingleStatementWithOC(accessor, ht);
			return (dv.Count != 0) ? dv[0] : null;
		}
	}
	protected void btnTopPage_Click(object sender, EventArgs e)
	{
		Response.Redirect("~/Form/Forum/ForumTop.aspx");
	}

	// ユーザ名の最大文字数
	protected void tbUserName_lengthValidate(object source, ServerValidateEventArgs args)
	{
		try
		{
			string i = args.Value;
			args.IsValid = (i.Length <= 50);
		}
		catch (Exception)
		{
			args.IsValid = false;
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