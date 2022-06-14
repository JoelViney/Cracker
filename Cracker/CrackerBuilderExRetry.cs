using System.Linq.Expressions;
using Cracker.Helpers;
using Cracker.Wrappers;

namespace Cracker
{
    /// <summary>
    /// CrackerBuilder with extra functionality for retry.
    /// </summary>
    public class CrackerBuilderExRetry : CrackerBuilder
    {
        internal RetryWrapperExBase? CommandEx
        {
            get 
            {
                if (base.Command == null)
                {
                    return null;
                }
                return (RetryWrapperExBase)base.Command;
            }
        }

        public CrackerBuilderExRetry(CrackerBuilder crackerBuilder) : base(crackerBuilder)
        {


        }

        public CrackerBuilderExRetry UnlessException<T>() where T : Exception
        {
            if (this.CommandEx == null)
            {
                throw new Exception("There is no function for the UnlessException to be applied to.");
            }

            this.CommandEx.AddUnlessException<T>();

            return this;
        }

        public CrackerBuilderExRetry UnlessException<T>(Expression<Func<T, bool>> predicate) where T : Exception
        {
            if (this.CommandEx == null)
            {
                throw new Exception("There is no function for the UnlessException to be applied to.");
            }

            this.CommandEx.AddUnlessException<T>(predicate);

            return this;
        }

        public CrackerBuilderExRetry WhenException<T>() where T : Exception
        {
            if (this.CommandEx == null)
            {
                throw new Exception("There is no function for the WhenException to be applied to.");
            }

            this.CommandEx.AddWhenException<T>();

            return this;
        }

        public CrackerBuilderExRetry WhenException<T>(Expression<Func<T, bool>> predicate) where T : Exception
        {
            if (this.CommandEx == null)
            {
                throw new Exception("There is no function for the WhenException to be applied to.");
            }

            this.CommandEx.AddWhenException<T>(predicate);

            return this;
        }

        public CrackerBuilderExRetry WhenResult<T>(Expression<Func<T, bool>> predicate)
        {
            if (this.CommandEx == null)
            {
                throw new Exception("There is no function for the WhenException to be applied to.");
            }

            this.CommandEx.AddWhenResult<T>(predicate);

            return this;
        }

        public CrackerBuilderExRetry WithDelayBetweenRetries(Expression<Func<int, TimeSpan>> predicate)
        {
            if (this.CommandEx == null)
            {
                throw new Exception("There is no function for the WhenException to be applied to.");
            }

            this.CommandEx.DelayExpression = predicate;
            return this;
        }
    }
}
