using OpenCLWrapper.CL.Internals;
using System;
using System.Collections.Generic;


namespace OpenCLWrapper.CL
{
    public class Platforms : ErrorHandler 
    {
        public IEnumerable<Platform> List { get; private set; }


        uint num_platforms = 0;


        public Platforms()
        {
            ThrowExceptionOnError(clGetPlatformIDs(0, null, out num_platforms));

            if (num_platforms == 0)
                throw new NotSupportedException("OpenCL not supported on this machine");
        }

        public IEnumerable<Platform> Scan()
        {
            IntPtr[] platformIds = new IntPtr[num_platforms];
            ThrowExceptionOnError(clGetPlatformIDs(num_platforms, platformIds, out num_platforms));

            List<Platform> platforms = new List<Platform>();
            foreach (IntPtr ptr in platformIds)
                platforms.Add(new Platform(ptr));

            List = platforms;

            return List;
        }
    }
}
