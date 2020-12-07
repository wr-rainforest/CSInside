using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;
using HtmlAgilityPack;

namespace CSInside
{
    public class PostContent : ICollection<Paragraph>
    {
        private List<Paragraph> paragraphs;

        public PostContent()
        {
            paragraphs = new List<Paragraph>();
        }

        #region ICollection
        public int Count => ((ICollection<Paragraph>)paragraphs).Count;

        public bool IsReadOnly => ((ICollection<Paragraph>)paragraphs).IsReadOnly;

        public void Add(Paragraph item)
        {
            ((ICollection<Paragraph>)paragraphs).Add(item);
        }

        public void Clear()
        {
            ((ICollection<Paragraph>)paragraphs).Clear();
        }

        public bool Contains(Paragraph item)
        {
            return ((ICollection<Paragraph>)paragraphs).Contains(item);
        }

        public void CopyTo(Paragraph[] array, int arrayIndex)
        {
            ((ICollection<Paragraph>)paragraphs).CopyTo(array, arrayIndex);
        }

        public bool Remove(Paragraph item)
        {
            return ((ICollection<Paragraph>)paragraphs).Remove(item);
        }

        public IEnumerator<Paragraph> GetEnumerator()
        {
            return ((IEnumerable<Paragraph>)paragraphs).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)paragraphs).GetEnumerator();
        }
        #endregion
    }
}
