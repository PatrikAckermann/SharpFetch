using System;
using System.Collections.Generic;
using System.Text;

namespace SharpFetch.Enums
{
    public enum BaseUrlTypeEnum
    {
        // Have for no BaseUrl, just domain part, more than that
        None = 0,
        // BaseUrl is just domain part, like https://example.com
        Domain = 1,
        // BaseUrl is more than domain part, like https://example.com/api
        MoreThanDomain = 2
    }
}
