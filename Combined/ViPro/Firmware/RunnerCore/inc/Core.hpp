#ifndef CORE_HPP
#define CORE_HPP

#include "Storage.hpp"
#include "IExecutable.hpp"

class Core : public IExecutable
{
    public:
        Core();
        void Execute();
    
    private:
        Storage storage;
};

#endif  //CORE_HPP
