using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moksy.Common
{
    /// <summary>
    /// Used to describe the kind of content that will be submitted. This is part of the condition and will validate accordingly. Imdb
    /// functionality is only allowed for Json and is implicitly set with the ToImdb() and FromImdb() calls. Otherwise, use AsText() and AsJson() in the 
    /// Condition. 
    /// </summary>
    public enum ContentKind
    {
        Text = 0,
        Json = 1
    }
}
