
using System;

public class Clock{
    float duration = 0;
    float fullDuration = 0;
    public Action Timeout = ()=>{};
    public Clock(float fullDuration, float duration = -1){
        this.fullDuration = fullDuration;
        if(duration < 0) duration = fullDuration;
        this.duration = duration;
    }
    public float GetDuration(){
        return duration;
    }
    public float GetProgress(){
        return duration / fullDuration;
    }
    public void Update(float elapsed){
        if(duration > 0){
            duration -= elapsed;
            if (duration < 0) duration = 0;
            if (duration == 0) Timeout();
        }
    }
    public void Reset(){
        duration = fullDuration;
    }
}