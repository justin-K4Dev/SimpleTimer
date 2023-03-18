//===========================================================================================
// Timer 처리자
//
// author : justin
// 
//===========================================================================================

public class SimpleTimer
{
  private DateTime m_begin_timestamp = DateTime.MinValue;
  private DateTime m_end_timestamp = DateTime.MinValue;

  private Int64 m_time_limit_msec = 0;

  private bool m_is_paused = false;
  private DateTime m_pause_timestamp = new DateTime();
  private Int64 m_pause_total_msec = 0;

  private bool m_active = false;

  public LogicTimer()
  {
  }

  public LogicTimer(Int64 timeLimitMSec)
  {
    setTimeLimitMSec(timeLimitMSec);
  }

  public void setTimer(Int64 timeLimitMSec)
  {
    setTimer(0, timeLimitMSec);
  }

  public void setTimer(Int64 beginUtcTick, Int64 timeLimitMSec)
  {
    m_begin_timestamp = beginUtcTick > 0 ? new DateTime(beginUtcTick) : DateTime.UtcNow;
    m_time_limit_msec = timeLimitMSec;

    m_is_paused = false;
    m_pause_total_msec = 0;

    m_end_timestamp = DateTime.MinValue;
  }

  public void activate(Int64 timeLimitMSec = 0)
  {
    m_active = true;
    if (timeLimitMSec != 0)
    {
      setTimer(0, timeLimitMSec);
    }
    else
    {
      reset(0);
    }
  }

  public void activate(Int64 beginUtcTick, Int64 timeLimitMSec = 0)
  {
    m_active = true;
    if (timeLimitMSec != 0)
    {
      setTimer(beginUtcTick, timeLimitMSec);
    }
    else
    {
      reset(beginUtcTick);
    }
  }

  public bool isActive()
  {
    return m_active;
  }

  public void deactivate()
  {
    m_active = false;
    m_end_timestamp = DateTime.UtcNow;
  }

  public void setTimeLimitMSec(Int64 limitMSec)
  {
    m_time_limit_msec = limitMSec;

    m_time_limit_msec = Math.Max(m_time_limit_msec, 0);
  }

  public void incTimeLimitMSec(Int64 incMSec)
  {
    m_time_limit_msec += incMSec;

    m_time_limit_msec = Math.Max(m_time_limit_msec, 0);
  }

  public Int64 getTimeLimitMSec()
  {
    return m_time_limit_msec;
  }

  public void reset()
  {
    setTimer(m_time_limit_msec);
  }

  public void reset(Int64 beginUtcTick)
  {
    setTimer(beginUtcTick, m_time_limit_msec);
  }

  public bool expired()
  {
    if (false == isActive())
    {
      return false;
    }

    if (true == isPause())
    {
      return false;
    }

    return (DateTime.UtcNow - m_begin_timestamp).TotalMilliseconds > (m_pause_total_msec + m_time_limit_msec);
  }

  public void pause()
  {
    if (isPause())
    {
      return;
    }

    m_is_paused = true;
    m_pause_timestamp = DateTime.UtcNow;
  }

  public bool isPause()
  {
    return m_is_paused;
  }

  public void unpause()
  {
    if (false == isPause())
    {
      return;
    }

    var pause_time = (DateTime.UtcNow - m_pause_timestamp).TotalMilliseconds;
    m_pause_total_msec += (Int64)pause_time;

    m_is_paused = false;
  }

  public Int64 getTotalTimeMSec()
  {
    if (!isActive())
    {
      return 0;
    }

    var end_time = m_begin_timestamp.AddMilliseconds(m_time_limit_msec);

    var total_time = (end_time - m_begin_timestamp).TotalMilliseconds;

    total_time = Math.Max(total_time, 0);

    return (Int64)total_time;
  }

  public Int64 getElapsedPauseTimeMSec()
  {
    if (false == isActive())
    {
      return 0;
    }

    var pause_time = m_pause_total_msec;

    if (true == isPause())
    {
      pause_time += (Int64)((DateTime.UtcNow - m_pause_timestamp).TotalMilliseconds);
    }

    pause_time = Math.Max(pause_time, 0);

    return (Int64)pause_time;
  }

  public bool isOn()
  {
    if (false == expired())
    {
      return false;
    }

    reset();

    return true;
  }

  public DateTime getStartedTime() => m_begin_timestamp;
  public DateTime getEndTime() => m_end_timestamp;

  public Int64 getRemainTimeMSec()
  {
    if (false == isActive())
    {
      return 0;
    }

    var pause_time = getElapsedPauseTimeMSec();

    TimeSpan time_span = DateTime.UtcNow - m_begin_timestamp;
    var remain_time = m_time_limit_msec + pause_time - (Int64)time_span.TotalMilliseconds;
    remain_time = Math.Max(remain_time, 0);

    return remain_time;
  }

  public Int64 getElapsedTimeMSec()
  {
    if (false == isActive())
    {
      return 0;
    }

    TimeSpan time_span = DateTime.UtcNow - m_begin_timestamp;
    return (Int64)time_span.TotalMilliseconds;
  }

}//end of SimpleTimer
