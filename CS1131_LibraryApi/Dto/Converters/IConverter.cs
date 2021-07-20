using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CS1131_LibraryApi.Dto.Converters
{
    public interface IConverter<TDomain, TDto>
    {
        public TDto Convert(TDomain obj);
        public TDomain Convert(TDto obj);
    }
}
