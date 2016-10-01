using System;
using System.Linq;

using PGSolutions.Monads.Test;

namespace PGSolutions.Monads.MonadTests {
    public class MixedMaybeTests {
        public MixedMaybeTests() {
            _addOne      = x => (x + 1);
            _concatEight = i => string.Format("{0}eight",i);
        }

        readonly Func<int, int>     _addOne;
        readonly Func<int,string>   _concatEight;

        public void MonadLaw3MaybeMixed() {
            var M = 4 as int?;

            // All four should be equivalent
            var lhs = from m in M from a in _addOne select _concatEight(a(m));
            var rhs = from u in M.Select(_addOne) from v in _concatEight(u) select v;

            lhs = M.SelectMany(m => _addOne, (m,a) => _concatEight(a(m)));
            rhs = M.Select(_addOne).SelectMany(_concatEight, (u,v) => v);
        }
    }
}
