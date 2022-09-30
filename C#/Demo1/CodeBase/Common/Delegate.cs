namespace Common
{
    public delegate bool SortDelegate<Tcurrent, Tnext>(Tcurrent currentItem, Tnext nextItem);
    public delegate bool SortDelegate(object currentItem, object nextItem);
}
