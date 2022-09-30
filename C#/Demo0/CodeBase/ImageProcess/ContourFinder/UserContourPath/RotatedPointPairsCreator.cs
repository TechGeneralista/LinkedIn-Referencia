using Common;
using Common.NotifyProperty;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ImageProcess.ContourFinder.UserContourPath
{
    public class RotatedPointPairsCreator
    {
        public INonSettableObservableProperty<bool> RotatedPointPairsChanged { get; } = new ObservableProperty<bool>(true);


        public RotatedPointPairs[] RotatedPointPairs
        {
            get
            {
                RotatedPointPairsChanged.ForceSet(false);
                return rotatedPointPairs;
            }

            private set
            {
                rotatedPointPairs = value;
                RotatedPointPairsChanged.ForceSet(true);
            }
        }


        RotatedPointPairs[] rotatedPointPairs = new RotatedPointPairs[0];


        public void Create(PointPair[] originPointPairs, int rotateToleranceMinus, int rotateTolerancePlus)
        {
            if (originPointPairs.IsNull() || originPointPairs.Length == 0)
            {
                RotatedPointPairs = new RotatedPointPairs[0];
                return;
            }

            List<RotatedPointPairs> rotatedPointPairsList = new List<RotatedPointPairs>();
            object lockObject = new object();

            Parallel.For(rotateToleranceMinus, (rotateTolerancePlus + 1), i =>
            {
                RotatedPointPairs rotatedPointPairs = new RotatedPointPairs(i, originPointPairs);
                lock (lockObject) { rotatedPointPairsList.Add(rotatedPointPairs); }
            });

            RotatedPointPairs = rotatedPointPairsList.ToArray();
        }
    }
}