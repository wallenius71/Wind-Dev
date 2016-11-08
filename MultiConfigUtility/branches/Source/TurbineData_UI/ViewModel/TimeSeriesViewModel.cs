using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using TurbineDataUtility.Model;
using System.Collections.ObjectModel;



namespace Analysis_UI
{
    public class TimeSeriesViewModel:ViewModelBase 
    {
        ObservableCollection<TagGroup> _tags=new ObservableCollection<TagGroup> ();
        List<int> _groupBySegement;
        List<Tag> _sourceTag;


        public TimeSeriesViewModel(List<Tag> tags)
        {
            //puts tags into groups according to criteria 
            Tags = new ObservableCollection<TagGroup>();


            
        }
        public TimeSeriesViewModel(List<Tag> tags, List<int> groupbysegment)
        {
            _groupBySegement = groupbysegment ;
            _sourceTag = tags;
            setTagGroups( );
        }

        public ObservableCollection<TagGroup> Tags
        {
            get { return _tags;}
            set
            {
                _tags = value;
                
            }
        }

        public void setTagGroups()
        {
            var groupBys = new List<Func<Tag, string>>();

            foreach (int i in _groupBySegement)
            {
                
                ConstantExpression ce = Expression.Constant(i, typeof(int));
                ParameterExpression tag = Expression.Parameter(typeof(Tag), "tag");
                MethodCallExpression e = Expression.Call(tag, typeof(Tag).GetMethod("TagNameElement", new Type[] { typeof(int) }), ce);

                var f = Expression.Lambda<Func<Tag, string>>(e, new ParameterExpression[] { tag });
                
                groupBys.Add((Func<Tag,string>) f.Compile());
                //Console.WriteLine(f.ToString());
            }

            var result = _sourceTag.Where(t => t.TagNameElement(8) == "Avg" && t.TagNameElement(4) == "DAT")
                .GroupByMany(groupBys.ToArray());
                

           
           ParseGroupResult(result, new List<string>());

            
        }
        private void ParseGroupResult(IEnumerable<GroupResult> results, List<string> headings)
        {
            //call recursively til we are at the leaf level
            IEnumerable<GroupResult> localResult = results;
            
            if (localResult.First().SubGroups != null)
            {
                foreach (GroupResult gr in localResult)
                {   
                    //if we are at the second to lst group only 
                    if(gr.SubGroups.First().SubGroups ==null)
                    headings.Add(gr.Key.ToString ());

                    ParseGroupResult(gr.SubGroups,headings);
                    
                }

            }
            else
            {       string title=string.Empty;
                    
                foreach (GroupResult gr in localResult )
                    {
                        headings.Add(gr.Key.ToString());
                        
                    }
                foreach (string header in headings)
                {
                    title+=header + " ";
                    
                }
                
                headings.Clear();

                foreach (GroupResult gr in localResult)
                {
                    TagGroup tg = new TagGroup() { Key = title };
                    foreach (Tag t in gr.Items)
                    {
                        
                        tg.Add(t);
                    }
                    Tags.Add(tg);


                    
                }
            }
        
                

     }
        
        
    }
    
    public static class MyEnumerableExtensions
    {
        //public static IEnumerable<GroupResult> GroupByMany<TElement>(
        //    this IEnumerable<TElement> elements, params string[] groupSelectors)
        //{
        //    var selectors = new List<Func<TElement, object>>(groupSelectors.Length);

        //    foreach (var selector in groupSelectors)
        //    {
        //        LambdaExpression l =
        //            DynamicExpression.ParseLambda(typeof(TElement), typeof(object), selector);
        //        selectors.Add((Func<TElement, object>)l.Compile());
        //    }
        //    return elements.GroupByMany(selectors.ToArray());
        //}

        public static IEnumerable<GroupResult> GroupByMany(
            this IEnumerable<Tag> elements, params Func<Tag, string>[] groupSelectors)
        {
            if (groupSelectors.Length > 0)
            {
                var selector = groupSelectors.First();
                var nextSelectors = groupSelectors.Skip(1).ToArray(); //reduce the list recursively until zero
                var result=
                    elements.GroupBy(selector).Select(
                        g => new GroupResult
                        {
                            
                            Key = g.Key,
                            Count = g.Count(),
                            Items = g,
                            SubGroups = g.GroupByMany(nextSelectors)
                        });
                
                return result;
            }
            else
                return null;
        }

       
    }
    
    

}
