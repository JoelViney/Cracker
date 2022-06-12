using System.Linq.Expressions;
using Cracker.Helpers;
using Cracker.Wrappers;

namespace Cracker
{
    /// <summary>
    /// CrackerBuilder with exception handling from the last command passed in.
    /// </summary>
    public class CrackerBuilderEx : CrackerBuilder
    {
        internal WrapperExBase CommandEx
        {
            get 
            { 
                return (WrapperExBase)base.Command;
            }
        }

        public CrackerBuilderEx(CrackerBuilder crackerBuilder) : base(crackerBuilder)
        {


        }

        public CrackerBuilderEx UnlessException<T>() where T : Exception
        {
            if (this.Command == null)
            {
                throw new Exception("There is no function for the UnlessException to be applied to.");
            }

            this.CommandEx.AddUnlessException<T>();

            return this;
        }

        public CrackerBuilderEx UnlessException<T>(Expression<Func<T, bool>> predicate) where T : Exception
        {
            if (this.Command == null)
            {
                throw new Exception("There is no function for the UnlessException to be applied to.");
            }

            this.CommandEx.AddUnlessException<T>(predicate);

            return this;
        }

        public CrackerBuilderEx WhenException<T>() where T : Exception
        {
            if (this.Command == null)
            {
                throw new Exception("There is no function for the WhenException to be applied to.");
            }

            this.CommandEx.AddWhenException<T>();

            return this;
        }

        public CrackerBuilderEx WhenException<T>(Expression<Func<T, bool>> predicate) where T : Exception
        {
            if (this.Command == null)
            {
                throw new Exception("There is no function for the WhenException to be applied to.");
            }

            this.CommandEx.AddWhenException<T>(predicate);

            return this;
        }

        public CrackerBuilderEx WhenResult<T>(Expression<Func<T, bool>> predicate)
        {
            if (this.Command == null)
            {
                throw new Exception("There is no function for the WhenException to be applied to.");
            }

            this.CommandEx.AddWhenResult<T>(predicate);

            return this;
        }
    }
}
