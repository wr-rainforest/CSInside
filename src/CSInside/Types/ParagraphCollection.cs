using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;

namespace CSInside
{
    /// <summary>
    /// 게시글의 단락 집합을 나타냅니다.
    /// </summary>
    public class ParagraphCollection : ICollection<Paragraph>
    {
        private List<Paragraph> paragraphs;


        public ParagraphCollection()
        {
            paragraphs = new List<Paragraph>();
        }

        #region ICollection
        public int Count => paragraphs.Count;

        public bool IsReadOnly => false;

        public void Add(Paragraph item) => paragraphs.Add(item);

        public void Clear() => paragraphs.Clear();

        public bool Contains(Paragraph item) => paragraphs.Contains(item);

        public void CopyTo(Paragraph[] array, int arrayIndex) => paragraphs.CopyTo(array, arrayIndex);

        public bool Remove(Paragraph item) => paragraphs.Remove(item);

        public IEnumerator<Paragraph> GetEnumerator() => paragraphs.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => paragraphs.GetEnumerator();
        #endregion
    }
}
