#include <vector>
#include <memory>
#include <iostream>
#include <optional>

using namespace std;

struct HasInputTriggerMethod {
    virtual ~HasInputTriggerMethod() {}
    virtual void trigger() = 0;
};

struct HasGetOutputMethod {
    virtual ~HasGetOutputMethod() {}
    virtual optional<int32_t> get_output() = 0;
};

class PrintValueModule : public HasInputTriggerMethod {
public:
    optional<shared_ptr<HasGetOutputMethod>> input;

    void trigger() {
        if (input.has_value())
        {
            optional<int32_t> prevModuleOutput = input.value()->get_output();
            if(prevModuleOutput.has_value())
                cout << "the value is: " << prevModuleOutput.value() << endl;
            else
                cout << "no value was received for the input!" << endl;
        }
        else
            cout << "input is empty" << endl;
    }

};

class ValueModule : public HasGetOutputMethod{
private:
    int32_t value;

public:
    ValueModule(int32_t value) {
        this->value = value;
    }

    optional<int32_t> get_output() {
        return value;
    }
};

class ValueAdderModule : public HasGetOutputMethod {
public:
    optional<shared_ptr<HasGetOutputMethod>> input0;
    optional<shared_ptr<HasGetOutputMethod>> input1;
    
    optional<int32_t> get_output() {
        
        int32_t value0;
        int32_t value1;

        if (input0.has_value())
        {
            optional<int32_t> result0 = input0.value()->get_output();
            if (result0.has_value())
                value0 = result0.value();
            else
            {
                cout << "no value was received for the input0!" << endl;
                return nullopt;
            }
        }
        else
        {
            cout << "input0 is empty" << endl;
            return nullopt;
        }

        if (input1.has_value())
        {
            optional<int32_t> result1 = input1.value()->get_output();
            if (result1.has_value())
                value1 = result1.value();
            else
            {
                cout << "no value was received for the input1!" << endl;
                return nullopt;
            }
        }
        else
        {
            cout << "input1 is empty" << endl;
            return nullopt;
        }

        return value0 + value1;
    }
};




int main()
{
    auto valueModule0 = make_shared<ValueModule>(10);
    auto valueModule1 = make_shared<ValueModule>(15);
    auto addModule = make_shared<ValueAdderModule>();
    auto printValueModule = make_shared<PrintValueModule>();

    addModule->input0 = valueModule0;
    addModule->input1 = valueModule1;
    printValueModule->input = addModule;
    printValueModule->trigger();

    return 0;
}
