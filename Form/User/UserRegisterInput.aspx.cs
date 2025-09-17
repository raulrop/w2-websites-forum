using System;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Common.Sql;
using w2.Common.Web;

public partial class Form_User_UserRegisterInput : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		lErrorMessage.Text = "";
	}
	protected void btnUserRegister_Click(object sender, EventArgs e)
	{
		var data = Get_login_id(tbLoginId.Text);
		if (data != null)
		{
			if (data["delete_flg"].ToString() == "1")
			{
				lErrorMessage.Text = HtmlSanitizer.HtmlEncode(data["login_id"].ToString() + "は退会済みです。");
				return;
			}
			lErrorMessage.Text = HtmlSanitizer.HtmlEncode(data["login_id"].ToString() + "は既に利用されているログインIDです");
			return;
		}
		var ht = new Hashtable
		{
			{"type", "Register"},
			{"name", tbUserName.Text},
			{"login_id", tbLoginId.Text},
			{"password", tbPassword.Text}
		};
		Session["param"] = ht;
		Response.Redirect("~/Form/User/UserRegisterConfirm.aspx");
	}

	protected void btnReturn_Click(object sender, EventArgs e)
	{
		Response.Redirect("~/Default.aspx");
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