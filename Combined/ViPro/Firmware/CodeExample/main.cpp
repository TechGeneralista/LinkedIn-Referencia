#include <stdint.h>
#include <chrono>
#include <iostream>

using std::cout; using std::endl;
using std::chrono::duration_cast;
using std::chrono::milliseconds;
using std::chrono::seconds;
using std::chrono::system_clock;

class IExecutable
{
    public:
        virtual ~IExecutable() = default;
        virtual void Execute() = 0;
};

class TimerOn : public IExecutable
{
    public:
        bool Enable = false;
        uint64_t Delay = 0;
        uint64_t TickAt = 0;
        bool IsRun = false;
        bool Tick = false;

        void Execute()
        {
            if(Enable)
            {
                if(!IsRun)
                {
                    IsRun = true;
                    TickAt = duration_cast<milliseconds>(system_clock::now().time_since_epoch()).count() + Delay;
                }
                else
                    Tick = TickAt <= duration_cast<milliseconds>(system_clock::now().time_since_epoch()).count();
            }
            else
            {
                IsRun = false;
                TickAt = 0;
                Tick = 0;
            }
        }
};

class CycleCounter : public IExecutable
{
    public:
        bool EndOfMeasure = false;

        void Execute()
        {
            if(EndOfMeasure)
            {
                EndOfMeasure = false;

                if(counter != 0)
                {
                    cout << counter << "cps (" << ((double)1/counter) * 1000 * 1000 << "ns)" << endl;
                    counter = 0;
                }
            }
            else
            {
                counter += 1;
            }
        }

    private:
        uint64_t counter = 0;
};



int main(int, char**) 
{
    TimerOn t0;
    t0.Enable = true;
    t0.Delay = 500;

    TimerOn t1;
    t1.Delay = 500;

    CycleCounter cc;

    while(true)
    {
        t0.Execute();

        if(t0.Tick)
        {
            t0.Enable = false;
            t1.Enable = true;
        }

        t1.Execute();

        if(t1.Tick)
        {
            t1.Enable = false;
            t0.Enable = true;
            cc.EndOfMeasure = true;
        }

        cc.Execute();
    }
}
