using SpiderFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatiN.Core;

namespace SpiderTasks.zhaopindotcom
{
    public class ZhaopinListTask:Task
    {
        const string listUrl = "http://rd2.zhaopin.com/s/loginmgr/login.asp";

        int lastPageNum = 0;

        void Login(Browser ie)
        {
            var tUsername = ie.TextField(Find.ByName("username"));
            tUsername.TypeText("dwed42779981");
            var tPwd = ie.TextField(Find.ByName("password"));
            tPwd.TypeText("selerant7353");
            ie.WaitForComplete(15);
            //var tValidate = ie.TextField("Validate");
            //tValidate.WaitUntil<TextField>(e => !string.IsNullOrEmpty(e.Text)&&e.Text.Length==4);
            ie.Button("loginbutton").Click();

            ie.WaitUntilContainsText("公司编号：");            
        }

        public override void DoStuff()
        {
            WatinContentExtractor ce = new WatinContentExtractor(new IE());
            if (!ce.Goto(listUrl))
                return;
            var ie = (Browser)ce.Instance;
            Login(ie);
            //进入搜索页面
            ie.GoTo("http://rdsearch.zhaopin.com/Home/SearchByCustom?source=rd");

            var tKeyWord = ie.TextField("SF_1_1_1");
            tKeyWord.TypeText("QA");    //TODO: use param instead
            var tSubmit = ie.Div("searchSubmit");
            tSubmit.Click();

            ie.WaitUntilContainsText("您搜索的是：");

            string t1= ie.Span("rd-resumelist-pageNum").Text;
            lastPageNum = Int32.Parse(t1.Split('/')[1]);

        }
    }
}
