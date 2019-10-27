using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class DateComponent : MonoBehaviour
{
    [Tooltip("起始年份")]
    public int startYear;
    [Tooltip("末尾年份")]
    public int endYear;

    public IMCSelectBox yearObject;//年
    public IMCSelectBox monthObject;//月
    public IMCSelectBox dateObject;//日

    public bool playOnAwake = false;

    private List<string> Years = new List<string>();
    private List<string> Months = new List<string>();
    private List<string> Days = new List<string>();

    private List<string> LeapMonths = new List<string>();

    private List<string> BigMonthDate = new List<string>();//大月
    private List<string> SmallMonthDate = new List<string>();//小月
    private List<string> OrdinaryFebruaryDate = new List<string>();//平年二月
    private List<string> BissextileMonthdate = new List<string>();//闰年二月
    private bool monthToggle = false;
    private bool yearToggle = false;
    void Start()
    {
        if (playOnAwake)
        {
            Show();
        }
    }
    //void Update()
    //{
    //    if (automatic)
    //    {

    //        if (!monthObject.GetSelectBoxState())// 获取当前状态，不动的话就执行里面的代码
    //        {
    //            if (monthToggle)
    //            {
    //                CountMonthDate();
    //                monthToggle = false;
    //                return;
    //            }
    //        }
    //        else if (monthObject.GetSelectBoxState())//获取当前状态，动的话就返回true
    //        {
    //            monthToggle = true;
    //        }

    //        if (!yearObject.GetSelectBoxState())// 获取当前状态，不动的话就执行里面的代码
    //        {
    //            if (yearToggle)
    //            {
    //                CountYearDate();
    //                yearToggle = false;
    //                return;
    //            }
    //        }
    //        else if (yearObject.GetSelectBoxState())//获取当前状态，动的话就返回true
    //        {
    //            yearToggle = true;
    //        }
    //    }
    //}

    private bool? springbackSwitch1 = null;
    private bool springbackSwitch2 = false;
    private bool springbackSwitch3 = false;
    private bool automatic = false;
    private DateTime dateLegal;
    private int dateContentListCount = 0;
    void FixedUpdate()
    {

        if (yearObject.m_CenterObject.alpha < 1 || monthObject.m_CenterObject.alpha < 1 || dateObject.m_CenterObject.alpha < 1)
        {
            dateContentListCount = dateObject.m_contentList.Count;//需要知道当月有多少天
            springbackSwitch3 = true;
            springbackSwitch1 = true;
            springbackSwitch2 = true;
        }
        if (springbackSwitch3)
        {
            if (springbackSwitch1 == true)
            {
                if (springbackSwitch2)
                {
                    if (!yearObject.GetSelectBoxState() && !monthObject.GetSelectBoxState() && !dateObject.GetSelectBoxState())
                    {
                        springbackSwitch1 = false;
                        springbackSwitch2 = false;
                    }
                }
            }
            else if (springbackSwitch1 == false)
            {
                string[] tempStrs = GetDateComponentValue().Split('-');
                dateLegal = new DateTime(Convert.ToInt16(tempStrs[0]), Convert.ToInt16(tempStrs[1]), Convert.ToInt16(tempStrs[2]));
                if (dateLegal > DateTime.Now)
                {
                    automatic = false;
                    yearObject.Refresh(CustomDate(DateTime.Now.Year, startYear, endYear));
                    monthObject.Refresh(CustomDate(DateTime.Now.Month, 1, 12));

                    //CancelInvoke("InvokeCountDate");
                    //Invoke("InvokeCountDate", 0.05f);
                    automatic = true;
                    dateObject.Refresh(CustomDate(DateTime.Now.Day, 1, dateContentListCount));
                    springbackSwitch1 = true;
                    springbackSwitch3 = false;
                    //automatic = true;
                }
                else if (dateLegal.Year<(DateTime.Now.Year-50))
                {
                     automatic = false;
                    yearObject.Refresh(CustomDate(DateTime.Now.Year-50, startYear, endYear));
                    monthObject.Refresh(CustomDate(1, 1, 12));

                    //CancelInvoke("InvokeCountDate");
                    //Invoke("InvokeCountDate", 0.05f);
                    automatic = true;
                    dateObject.Refresh(CustomDate(1, 1, dateContentListCount));
                    springbackSwitch1 = true;
                    springbackSwitch3 = false;
                }
            }
        }

            if (automatic)
            {
                automatic = false;
                return;
            }

            if (!monthObject.GetSelectBoxState())// 获取当前状态，不动的话就执行里面的代码
            {
                if (monthToggle)
                {
                    CountMonthDate();
                    monthToggle = false;
                    return;
                }
            }
            else if (monthObject.GetSelectBoxState())//获取当前状态，动的话就返回true
            {
                monthToggle = true;
            }

            if (!yearObject.GetSelectBoxState())// 获取当前状态，不动的话就执行里面的代码
            {
                if (yearToggle)
                {
                    CountYearDate();
                    yearToggle = false;
                    return;
                }
            }
            else if (yearObject.GetSelectBoxState())//获取当前状态，动的话就返回true
            {
                yearToggle = true;
            }
    }

    void InvokeCountDate()
    {
        //dateObject.Refresh(CustomDate(DateTime.Now.Day, 1, dateContentListCount));
        automatic = true;
    }

    void CountMonthDate()//计算月
    {
        if (LeapMonths.Contains(monthObject.SelectValue))
        {
            dateObject.Refresh(BigMonthDate);
        }
        else
        {
            if (monthObject.SelectValue == "2")
            {
                if (Convert.ToInt32(yearObject.SelectValue) % 4 == 0)
                {
                    dateObject.Refresh(BissextileMonthdate);
                }
                else
                {
                    dateObject.Refresh(OrdinaryFebruaryDate);
                }
                return;
            }
            dateObject.Refresh(SmallMonthDate);
        }
    }
    void CountYearDate()//计算年
    {
        if (yearObject.SelectValue != "")
        {
            if (Convert.ToInt32(yearObject.SelectValue) % 4 == 0)//计算当时的年份是否是闰年
            {                                                   //闰月
                if (monthObject.SelectValue == "2")
                {
                    dateObject.Refresh(BissextileMonthdate);
                }
            }
            else
            {                                                   //平月
                if (monthObject.SelectValue == "2")
                {
                    dateObject.Refresh(OrdinaryFebruaryDate);
                }
            }
        }
    }
    /// <summary>
    /// 获取日期 以"-"分割
    /// </summary>
    public string GetDateComponentValue()
    {
        if (yearObject.SelectValue != "" && monthObject.SelectValue != "" && dateObject.SelectValue != "")
        {
            return yearObject.SelectValue + "-" + monthObject.SelectValue + "-" + dateObject.SelectValue;
        }
        return "";
    }
    public void Show(int start = 0, int end = 0)
    {
        if (showNumber == 0)
        {
            Initialize();
            if (showSwitch)
            {
                if (start != 0 || end != 0)
                {
                    startYear = start;
                    endYear = start;
                }
                else
                {
                    startYear = DateTime.Now.Year + 10;
                    endYear = startYear - 50;
                }
                //if (DateTime.Now.Year-startYear<10)
                //{
                //    startYear += 5;
                //    endYear = DateTime.Now.Year - 50;
                //}
                yearObject.Create(yearObject.sizeDelta.x, yearObject.sizeDelta.y, yearObject.sizeDelta.x, 120, 70, Years = SetYearMonthDate(startYear, endYear));
                monthObject.Create(monthObject.sizeDelta.x, monthObject.sizeDelta.y, monthObject.sizeDelta.x, 120, 70, Months);
                dateObject.Create(dateObject.sizeDelta.x, dateObject.sizeDelta.y, dateObject.sizeDelta.x, 120, 70, BigMonthDate);
                //yearObject.Create(yearObject.sizeDelta.x, yearObject.sizeDelta.y, yearObject.sizeDelta.x, 120, 70, Years = CustomDate(2017, startYear, endYear));
                //monthObject.Create(monthObject.sizeDelta.x, monthObject.sizeDelta.y, monthObject.sizeDelta.x, 120, 70, Years = CustomDate(6, 1, 12));
                //dateObject.Create(dateObject.sizeDelta.x, dateObject.sizeDelta.y, dateObject.sizeDelta.x, 120, 70, Years = CustomDate(5, 1, 30));
                showSwitch = false;
            }
            showNumber++;
        }
    }
    private int showNumber = 0;
    private void Initialize()
    {
        if (LeapMonths.Count == 0)
        {
            LeapMonths.Add("1");
            LeapMonths.Add("3");
            LeapMonths.Add("5");
            LeapMonths.Add("7");
            LeapMonths.Add("8");
            LeapMonths.Add("10");
            LeapMonths.Add("12");
        }

        Months = SetYearMonthDate(1, 12);
        BigMonthDate = SetYearMonthDate(1, 31);
        SmallMonthDate = SetYearMonthDate(1, 30);
        OrdinaryFebruaryDate = SetYearMonthDate(1, 28);
        BissextileMonthdate = SetYearMonthDate(1, 29);
    }
    private bool showSwitch = true;
    /// <summary>
    /// 自定义日期
    /// </summary>
    public void Set(int year, int month, int day)
    {
        if (showNumber == 0)
        {
            Initialize();

            yearObject.Create(yearObject.sizeDelta.x, yearObject.sizeDelta.y, yearObject.sizeDelta.x, 120, 70, Years = CustomDate(year, startYear, endYear));
            monthObject.Create(monthObject.sizeDelta.x, monthObject.sizeDelta.y, monthObject.sizeDelta.x, 120, 70, CustomDate(month, 1, 12));


            if (year % 4 == 0)//计算当时的年份是否是闰年
            {                                                   //闰月
                if (month.ToString() == "2")
                {
                    dateObject.Create(dateObject.sizeDelta.x, dateObject.sizeDelta.y, dateObject.sizeDelta.x, 120, 70, CustomDate(day, 1, 29));
                }
                else if (LeapMonths.Contains(month.ToString()))//1   3   5   
                {
                    dateObject.Create(dateObject.sizeDelta.x, dateObject.sizeDelta.y, dateObject.sizeDelta.x, 120, 70, CustomDate(day, 1, 31));
                }
                else
                {
                    dateObject.Create(dateObject.sizeDelta.x, dateObject.sizeDelta.y, dateObject.sizeDelta.x, 120, 70, CustomDate(day, 1, 30));
                }
            }
            else
            {                                                   //平月
                if (month.ToString() == "2")
                {
                    dateObject.Create(dateObject.sizeDelta.x, dateObject.sizeDelta.y, dateObject.sizeDelta.x, 120, 70, CustomDate(day, 1, 28));
                }
                else if (LeapMonths.Contains(month.ToString()))//1   3   5   
                {
                    dateObject.Create(dateObject.sizeDelta.x, dateObject.sizeDelta.y, dateObject.sizeDelta.x, 120, 70, CustomDate(day, 1, 31));
                }
                else
                {
                    dateObject.Create(dateObject.sizeDelta.x, dateObject.sizeDelta.y, dateObject.sizeDelta.x, 120, 70, CustomDate(day, 1, 30));
                }
            }
        }
    }

    private List<string> CustomDate(int customNumb, int formNumb, int toNumb)//自定义日期型数据
    {
        List<string> tempStrArr = new List<string>();
        if (formNumb < toNumb)
        {
            //2008~2017
            for (int i = customNumb; i <= toNumb; i++)
            {
                tempStrArr.Add(i.ToString());
            }
            for (int i = formNumb; i < customNumb; i++)
            {
                tempStrArr.Add(i.ToString());
            }
        }
        else
        {
            //2017~2008
            for (int i = customNumb; i >= toNumb; i--)
            {
                tempStrArr.Add(i.ToString());
            }
            for (int i = formNumb; i > customNumb; i--)
            {
                tempStrArr.Add(i.ToString());
            }
        }

        return tempStrArr;
    }

    private List<string> SetYearMonthDate(int formNumb, int toNumb)
    {
        List<string> stringList = new List<string>();
        if (formNumb == toNumb)
        {
            formNumb = 1950;
            toNumb = DateTime.Now.Year;
        }
        else if (formNumb < toNumb)
        {
            for (int i = formNumb; i <= toNumb; i++)
            {
                stringList.Add(i.ToString());
            }
        }
        else
        {
            for (int i = formNumb; i >= toNumb; i--)
            {
                stringList.Add(i.ToString());
            }
        }
        return stringList;
    }

}
