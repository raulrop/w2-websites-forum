using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Form_User_UserRegisterComplete : System.Web.UI.Page
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
	}

	protected void btnTopPage_Click(object sender, EventArgs e)
	{
		Response.Redirect("~/Form/Forum/ForumTop.aspx");
	}

	protected void tThreeSeconds_Tick(object sender, EventArgs e)
	{
		Response.Redirect("~/Form/Forum/ForumTop.aspx");
	}
}