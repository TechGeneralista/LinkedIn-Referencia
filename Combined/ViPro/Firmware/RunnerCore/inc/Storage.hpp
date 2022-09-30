#ifndef STORAGE_HPP
#define STORAGE_HPP

#include <map>
#include <string>
#include <stdint.h>

class Storage
{
    public:
        std::map<std::string, int8_t> Int8s;
        std::map<std::string, int16_t> Int16s;
        std::map<std::string, int32_t> Int32s;
        std::map<std::string, int64_t> Int64s;
        std::map<std::string, uint8_t> UInt8s;
        std::map<std::string, uint16_t> UInt16s;
        std::map<std::string, uint32_t> UInt32s;
        std::map<std::string, uint64_t> UInt64s;
        std::map<std::string, bool> Bits;
        std::map<std::string, float> Floats;
        std::map<std::string, double> Doubles;
};

#endif //STORAGE_HPP
