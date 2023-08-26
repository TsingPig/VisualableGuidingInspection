using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Date
{
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6,
    Sunday = 7
}
public enum DayPeriod
{
    BeforeDown = 1,
    Morning,
    Afternoon,
    Evening
}
public class Period
{
    private Date _date;
    private DayPeriod _dayPeriod;
    public Date Date { get => _date; }
    public DayPeriod DayPeriod { get => _dayPeriod; }
    public Period()
    {
        _date = Date.Monday;
        _dayPeriod = DayPeriod.BeforeDown;
    }
    public Period(Date date, DayPeriod dayPeriod)
    {
        _date = date;
        _dayPeriod = dayPeriod;
    }
    public void MoveNext()
    {
        if (_dayPeriod == DayPeriod.Evening) // 如果当前时间段是 Evening
        {
            _dayPeriod = DayPeriod.BeforeDown; // 切换到下一天的 BeforeDown
            _date = (Date)(((int)_date % 7) + 1); // 切换到下一天
        }
        else
        {
            _dayPeriod = (DayPeriod)((int)_dayPeriod + 1); // 否则，切换到下一个时间段
        }
    }

    public string GetPeriod()
    {
        string day = "";
        string period = "";
        switch (_date)
        {
            case Date.Monday:
                day = "星期一";
                break;
            case Date.Tuesday:
                day = "星期二";
                break;
            case Date.Wednesday:
                day = "星期三";
                break;
            case Date.Thursday:
                day = "星期四";
                break;
            case Date.Friday:
                day = "星期五";
                break;
            case Date.Saturday:
                day = "星期六";
                break;
            case Date.Sunday:
                day = "星期日";
                break;
        }
        switch (_dayPeriod)
        {
            case DayPeriod.BeforeDown:
                period = "凌晨";
                break;
            case DayPeriod.Morning:
                period = "上午";
                break;
            case DayPeriod.Afternoon:
                period = "下午";
                break;
            case DayPeriod.Evening:
                period = "晚上";
                break;
        }
        return $"{day} {period}";
    }
}
public class PeriodManager : Singleton<PeriodManager>
{
    public const float DAY_PERIOD_DURATION = 3f;

    private float _currentTime;

    private Period _currentPeriod;

    private void Init()
    {
        _currentTime = 0;
        _currentPeriod = new Period();
        StartCoroutine(UpdateTime());
        Log.Info("初始化时间段：", _currentPeriod.GetPeriod());
    }
    private IEnumerator UpdateTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(DAY_PERIOD_DURATION);
            _currentPeriod.MoveNext();
        }
    }
    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    private void Update()
    {

    }
    public void LogPeriod()
    {
        Log.Info("当前时间段：", _currentPeriod.GetPeriod());
    }
}


