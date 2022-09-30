namespace ImageProcess.KernelSources
{
    public static class KernelSourceCenterPositionGetter
    {
        public static string FunctionName = "center_getter";

        public static string FunctionSource =
            @"

                __kernel void " + FunctionName + @"(global uchar* result, volatile global uint* counter, global uint* xPos, global uint* yPos) 
                {
                    int resultPos = get_global_id(0) + (get_global_id(1) * get_global_size(0)); 
                    
                    if(result[resultPos] == 1)
                    {
                        uint addr = atomic_inc(counter);
                        xPos[addr] = (uint)get_global_id(0);
                        yPos[addr] = (uint)get_global_id(1);
                    }
                }
                
            ";

        
    }
}
