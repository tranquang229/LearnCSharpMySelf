using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp2
{
    public class JsonDiffPatch
    {
        private readonly Options _options;

        public JsonDiffPatch()
          : this(new Options())
        {
        }

        public JsonDiffPatch(Options options) => this._options = options != null ? options : throw new ArgumentNullException(nameof(options));

        public JToken Diff(JToken left, JToken right)
        {
            if (left == null)
                left = (JToken)new JValue("");
            if (right == null)
                right = (JToken)new JValue("");
            if (left.Type == JTokenType.Object && right.Type == JTokenType.Object)
                return (JToken)this.ObjectDiff((JObject)left, (JObject)right);
            if (this._options.ArrayDiff == ArrayDiffMode.Efficient && left.Type == JTokenType.Array && right.Type == JTokenType.Array)
                return (JToken)this.ArrayDiff((JArray)left, (JArray)right);
            if (this._options.TextDiff == TextDiffMode.Efficient && left.Type == JTokenType.String && right.Type == JTokenType.String && ((long)left.ToString().Length > this._options.MinEfficientTextDiffLength || (long)right.ToString().Length > this._options.MinEfficientTextDiffLength))
            {
                diff_match_patch diffMatchPatch = new diff_match_patch();
                List<Patch> patchList = diffMatchPatch.patch_make(left.ToObject<string>(), right.ToObject<string>());
                if (!patchList.Any<Patch>())
                    return (JToken)null;
                return (JToken)new JArray(new object[3]
                {
          (object) diffMatchPatch.patch_toText(patchList),
          (object) 0,
          (object) 2
                });
            }
            if (JToken.DeepEquals(left, right))
                return (JToken)null;
            return (JToken)new JArray(new object[2]
            {
        (object) left,
        (object) right
            });
        }

        public JToken Patch(JToken left, JToken patch)
        {
            if (patch == null)
                return left;
            if (patch.Type == JTokenType.Object)
            {
                JObject patch1 = (JObject)patch;
                JProperty jproperty = patch1.Property("_t");
                return left != null && left.Type == JTokenType.Array && jproperty != null && jproperty.Value.Type == JTokenType.String && jproperty.Value.ToObject<string>() == "a" ? (JToken)this.ArrayPatch((JArray)left, patch1) : (JToken)this.ObjectPatch(left as JObject, patch1);
            }
            if (patch.Type != JTokenType.Array)
                return (JToken)null;
            JArray jarray = (JArray)patch;
            if (jarray.Count == 1)
                return jarray[0];
            if (jarray.Count == 2)
                return jarray[1];
            if (jarray.Count != 3)
                throw new InvalidDataException("Invalid patch object");
            if (jarray[2].Type != JTokenType.Integer)
                throw new InvalidDataException("Invalid patch object");
            switch (jarray[2].Value<int>())
            {
                case 0:
                    return (JToken)null;
                case 2:
                    if (left.Type != JTokenType.String)
                        throw new InvalidDataException("Invalid patch object");
                    diff_match_patch diffMatchPatch = new diff_match_patch();
                    List<Patch> patchList = diffMatchPatch.patch_fromText(jarray[0].ToObject<string>());
                    if (!patchList.Any<Patch>())
                        throw new InvalidDataException("Invalid textline");
                    object[] objArray = diffMatchPatch.patch_apply(patchList, left.Value<string>());
                    return !((IEnumerable<bool>)(bool[])objArray[1]).Any<bool>((Func<bool, bool>)(x => !x)) ? (JToken)(string)objArray[0] : throw new InvalidDataException("Text patch failed");
                default:
                    throw new InvalidDataException("Invalid patch object");
            }
        }

        public JToken Unpatch(JToken right, JToken patch)
        {
            if (patch == null)
                return right;
            if (patch.Type == JTokenType.Object)
            {
                JObject patch1 = (JObject)patch;
                JProperty jproperty = patch1.Property("_t");
                return right != null && right.Type == JTokenType.Array && jproperty != null && jproperty.Value.Type == JTokenType.String && jproperty.Value.ToObject<string>() == "a" ? (JToken)this.ArrayUnpatch((JArray)right, patch1) : (JToken)this.ObjectUnpatch(right as JObject, patch1);
            }
            if (patch.Type != JTokenType.Array)
                return (JToken)null;
            JArray jarray = (JArray)patch;
            if (jarray.Count == 1)
                return (JToken)null;
            if (jarray.Count == 2)
                return jarray[0];
            if (jarray.Count != 3)
                throw new InvalidDataException("Invalid patch object");
            if (jarray[2].Type != JTokenType.Integer)
                throw new InvalidDataException("Invalid patch object");
            switch (jarray[2].Value<int>())
            {
                case 0:
                    return jarray[0];
                case 2:
                    if (right.Type != JTokenType.String)
                        throw new InvalidDataException("Invalid patch object");
                    diff_match_patch diffMatchPatch = new diff_match_patch();
                    List<Patch> source = diffMatchPatch.patch_fromText(jarray[0].ToObject<string>());
                    if (!source.Any<Patch>())
                        throw new InvalidDataException("Invalid textline");
                    List<Patch> patches = new List<Patch>();
                    for (int index = source.Count - 1; index >= 0; --index)
                    {
                        Patch patch2 = source[index];
                        Patch patch3 = new Patch()
                        {
                            length1 = patch2.length1,
                            length2 = patch2.length2,
                            start1 = patch2.start1,
                            start2 = patch2.start2
                        };
                        foreach (Diff diff in patch2.diffs)
                        {
                            if (diff.operation == Operation.DELETE)
                                patch3.diffs.Add(new Diff(Operation.INSERT, diff.text));
                            else if (diff.operation == Operation.INSERT)
                                patch3.diffs.Add(new Diff(Operation.DELETE, diff.text));
                            else
                                patch3.diffs.Add(diff);
                        }
                        patches.Add(patch3);
                    }
                    object[] objArray = diffMatchPatch.patch_apply(patches, right.Value<string>());
                    return !((IEnumerable<bool>)(bool[])objArray[1]).Any<bool>((Func<bool, bool>)(x => !x)) ? (JToken)(string)objArray[0] : throw new InvalidDataException("Text patch failed");
                default:
                    throw new InvalidDataException("Invalid patch object");
            }
        }

        public string Diff(string left, string right) => this.Diff(JToken.Parse(left ?? ""), JToken.Parse(right ?? ""))?.ToString();

        public string Patch(string left, string patch) => this.Patch(JToken.Parse(left ?? ""), JToken.Parse(patch ?? ""))?.ToString();

        public string Unpatch(string right, string patch) => this.Unpatch(JToken.Parse(right ?? ""), JToken.Parse(patch ?? ""))?.ToString();

        private JObject ObjectDiff(JObject left, JObject right)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));
            JObject jobject = new JObject();
            foreach (JProperty property in left.Properties())
            {
                JProperty lp = property;
                if (this._options.ExcludePaths.Count <= 0 || !this._options.ExcludePaths.Any<string>((Func<string, bool>)(p => p.Equals(lp.Path, StringComparison.OrdinalIgnoreCase))))
                {
                    JProperty jproperty = right.Property(lp.Name);
                    if (jproperty != null || (this._options.DiffBehaviors & DiffBehavior.IgnoreMissingProperties) != DiffBehavior.IgnoreMissingProperties)
                    {
                        if (jproperty == null)
                        {
                            jobject.Add((object)new JProperty(lp.Name, (object)new JArray(new object[3]
                            {
                (object) lp.Value,
                (object) 0,
                (object) 0
                            })));
                        }
                        else
                        {
                            JToken content = this.Diff(lp.Value, jproperty.Value);
                            if (content != null)
                                jobject.Add((object)new JProperty(lp.Name, (object)content));
                        }
                    }
                }
            }
            foreach (JProperty property in right.Properties())
            {
                if (left.Property(property.Name) == null && (this._options.DiffBehaviors & DiffBehavior.IgnoreNewProperties) != DiffBehavior.IgnoreNewProperties)
                    jobject.Add((object)new JProperty(property.Name, (object)new JArray((object)property.Value)));
            }
            return jobject.Properties().Any<JProperty>() ? jobject : (JObject)null;
        }

        private JObject ArrayDiff(JArray left, JArray right)
        {
            JObject jobject = JObject.Parse("{ \"_t\": \"a\" }");
            int index1 = 0;
            int num = 0;
            if (JToken.DeepEquals((JToken)left, (JToken)right))
                return (JObject)null;
            while (index1 < left.Count && index1 < right.Count && JToken.DeepEquals(left[index1], right[index1]))
                ++index1;
            while (num + index1 < left.Count && num + index1 < right.Count && JToken.DeepEquals(left[left.Count - 1 - num], right[right.Count - 1 - num]))
                ++num;
            if (index1 + num == left.Count)
            {
                for (int index2 = index1; index2 < right.Count - num; ++index2)
                    jobject[string.Format("{0}", (object)index2)] = (JToken)new JArray((object)right[index2]);
                return jobject;
            }
            if (index1 + num == right.Count)
            {
                for (int index3 = index1; index3 < left.Count - num; ++index3)
                    jobject[string.Format("_{0}", (object)index3)] = (JToken)new JArray(new object[3]
                    {
            (object) left[index3],
            (object) 0,
            (object) 0
                    });
                return jobject;
            }
            Lcs lcs = Lcs.Get(left.ToList<JToken>().GetRange(index1, left.Count - num - index1), right.ToList<JToken>().GetRange(index1, right.Count - num - index1));
            for (int index4 = index1; index4 < left.Count - num; ++index4)
            {
                if (lcs.Indices1.IndexOf(index4 - index1) < 0)
                    jobject[string.Format("_{0}", (object)index4)] = (JToken)new JArray(new object[3]
                    {
            (object) left[index4],
            (object) 0,
            (object) 0
                    });
            }
            for (int index5 = index1; index5 < right.Count - num; ++index5)
            {
                int index6 = lcs.Indices2.IndexOf(index5 - index1);
                if (index6 < 0)
                {
                    jobject[string.Format("{0}", (object)index5)] = (JToken)new JArray((object)right[index5]);
                }
                else
                {
                    int index7 = lcs.Indices1[index6] + index1;
                    int index8 = lcs.Indices2[index6] + index1;
                    JToken jtoken = this.Diff(left[index7], right[index8]);
                    if (jtoken != null)
                        jobject[string.Format("{0}", (object)index5)] = jtoken;
                }
            }
            return jobject;
        }

        private JObject ObjectPatch(JObject obj, JObject patch)
        {
            if (obj == null)
                obj = new JObject();
            if (patch == null)
                return obj;
            JObject jobject = (JObject)obj.DeepClone();
            foreach (JProperty property in patch.Properties())
            {
                JProperty jproperty = jobject.Property(property.Name);
                JToken patch1 = property.Value;
                if (patch1.Type == JTokenType.Array && ((JContainer)patch1).Count == 3 && patch1[(object)2].Value<int>() == 0)
                    jobject.Remove(property.Name);
                else if (jproperty == null)
                    jobject.Add((object)new JProperty(property.Name, (object)this.Patch((JToken)null, patch1)));
                else
                    jproperty.Value = this.Patch(jproperty.Value, patch1);
            }
            return jobject;
        }

        private JArray ArrayPatch(JArray left, JObject patch)
        {
            List<JProperty> jpropertyList1 = new List<JProperty>();
            List<JProperty> jpropertyList2 = new List<JProperty>();
            List<JProperty> jpropertyList3 = new List<JProperty>();
            foreach (JProperty property in patch.Properties())
            {
                if (!(property.Name == "_t"))
                {
                    JArray jarray = property.Value as JArray;
                    if (property.Name.StartsWith("_"))
                    {
                        if (jarray == null || jarray.Count != 3 || jarray[2].ToObject<int>() != 0 && jarray[2].ToObject<int>() != 3)
                            throw new Exception(string.Format("Only removal or move can be applied at original array indices. Context: {0}", (object)jarray));
                        jpropertyList1.Add(new JProperty(property.Name.Substring(1), (object)property.Value));
                        if (jarray[2].ToObject<int>() == 3)
                            jpropertyList2.Add(new JProperty(jarray[1].ToObject<int>().ToString(), (object)new JArray((object)left[int.Parse(property.Name.Substring(1))].DeepClone())));
                    }
                    else if (jarray != null && jarray.Count == 1)
                        jpropertyList2.Add(property);
                    else
                        jpropertyList3.Add(property);
                }
            }
            jpropertyList1.Sort((Comparison<JProperty>)((x, y) => int.Parse(x.Name).CompareTo(int.Parse(y.Name))));
            for (int index = jpropertyList1.Count - 1; index >= 0; --index)
            {
                JProperty jproperty = jpropertyList1[index];
                left.RemoveAt(int.Parse(jproperty.Name));
            }
            jpropertyList2.Sort((Comparison<JProperty>)((x, y) => int.Parse(y.Name).CompareTo(int.Parse(x.Name))));
            for (int index = jpropertyList2.Count - 1; index >= 0; --index)
            {
                JProperty jproperty = jpropertyList2[index];
                left.Insert(int.Parse(jproperty.Name), ((JArray)jproperty.Value)[0]);
            }
            foreach (JProperty jproperty in jpropertyList3)
            {
                JToken jtoken = this.Patch(left[int.Parse(jproperty.Name)], jproperty.Value);
                left[int.Parse(jproperty.Name)] = jtoken;
            }
            return left;
        }

        private JObject ObjectUnpatch(JObject obj, JObject patch)
        {
            if (obj == null)
                obj = new JObject();
            if (patch == null)
                return obj;
            JObject jobject = (JObject)obj.DeepClone();
            foreach (JProperty property in patch.Properties())
            {
                JProperty jproperty = jobject.Property(property.Name);
                JToken patch1 = property.Value;
                if (patch1.Type == JTokenType.Array && ((JContainer)patch1).Count == 1)
                    jobject.Remove(jproperty.Name);
                else if (jproperty == null)
                    jobject.Add((object)new JProperty(property.Name, (object)this.Unpatch((JToken)null, patch1)));
                else
                    jproperty.Value = this.Unpatch(jproperty.Value, patch1);
            }
            return jobject;
        }

        private JArray ArrayUnpatch(JArray right, JObject patch)
        {
            List<JProperty> jpropertyList1 = new List<JProperty>();
            List<JProperty> jpropertyList2 = new List<JProperty>();
            List<JProperty> jpropertyList3 = new List<JProperty>();
            foreach (JProperty property in patch.Properties())
            {
                if (!(property.Name == "_t"))
                {
                    JArray jarray = property.Value as JArray;
                    if (property.Name.StartsWith("_"))
                    {
                        if (jarray == null || jarray.Count != 3 || jarray[2].ToObject<int>() != 0 && jarray[2].ToObject<int>() != 3)
                            throw new Exception(string.Format("Only removal or move can be applied at original array indices. Context: {0}", (object)jarray));
                        JProperty jproperty = new JProperty(jarray[1].ToObject<int>().ToString(), (object)property.Value);
                        if (jarray[2].ToObject<int>() == 3)
                        {
                            jpropertyList2.Add(new JProperty(property.Name.Substring(1), (object)new JArray((object)right[jarray[1].ToObject<int>()].DeepClone())));
                            jpropertyList1.Add(jproperty);
                        }
                        else
                            jpropertyList2.Add(new JProperty(property.Name.Substring(1), (object)new JArray((object)jarray[0])));
                    }
                    else if (jarray != null && jarray.Count == 1)
                        jpropertyList1.Add(property);
                    else
                        jpropertyList3.Add(property);
                }
            }
            foreach (JProperty jproperty in jpropertyList3)
            {
                JToken jtoken = this.Unpatch(right[int.Parse(jproperty.Name)], jproperty.Value);
                right[int.Parse(jproperty.Name)] = jtoken;
            }
            jpropertyList1.Sort((Comparison<JProperty>)((x, y) => int.Parse(x.Name).CompareTo(int.Parse(y.Name))));
            for (int index = jpropertyList1.Count - 1; index >= 0; --index)
            {
                JProperty jproperty = jpropertyList1[index];
                right.RemoveAt(int.Parse(jproperty.Name));
            }
            jpropertyList2.Sort((Comparison<JProperty>)((x, y) => int.Parse(x.Name).CompareTo(int.Parse(y.Name))));
            foreach (JProperty jproperty in jpropertyList2)
                right.Insert(int.Parse(jproperty.Name), ((JArray)jproperty.Value)[0]);
            return right;
        }
    }

    public sealed class Options
    {
        public Options()
        {
            this.ArrayDiff = ArrayDiffMode.Efficient;
            this.TextDiff = TextDiffMode.Efficient;
            this.MinEfficientTextDiffLength = 50L;
        }

        public ArrayDiffMode ArrayDiff { get; set; }

        public TextDiffMode TextDiff { get; set; }

        public long MinEfficientTextDiffLength { get; set; }

        public List<string> ExcludePaths { get; set; } = new List<string>();

        public DiffBehavior DiffBehaviors { get; set; }
    }

    public enum ArrayDiffMode
    {
        Efficient,
        Simple,
    }

    public enum TextDiffMode
    {
        Efficient,
        Simple,
    }

    [Flags]
    public enum DiffBehavior
    {
        None = 0,
        IgnoreMissingProperties = 1,
        IgnoreNewProperties = 2,
    }

    public class diff_match_patch
    {
        public float Diff_Timeout = 1f;
        public short Diff_EditCost = 4;
        public float Match_Threshold = 0.5f;
        public int Match_Distance = 1000;
        public float Patch_DeleteThreshold = 0.5f;
        public short Patch_Margin = 4;
        private short Match_MaxBits = 32;
        private Regex BLANKLINEEND = new Regex("\\n\\r?\\n\\Z");
        private Regex BLANKLINESTART = new Regex("\\A\\r?\\n\\r?\\n");

        public List<Diff> diff_main(string text1, string text2) => this.diff_main(text1, text2, true);

        public List<Diff> diff_main(string text1, string text2, bool checklines)
        {
            DateTime deadline = (double)this.Diff_Timeout > 0.0 ? DateTime.Now + new TimeSpan((long)((double)this.Diff_Timeout * 1000.0) * 10000L) : DateTime.MaxValue;
            return this.diff_main(text1, text2, checklines, deadline);
        }

        private List<Diff> diff_main(
          string text1,
          string text2,
          bool checklines,
          DateTime deadline)
        {
            if (text1 == text2)
            {
                List<Diff> diffList = new List<Diff>();
                if (text1.Length != 0)
                    diffList.Add(new Diff(Operation.EQUAL, text1));
                return diffList;
            }
            int num1 = this.diff_commonPrefix(text1, text2);
            string text3 = text1.Substring(0, num1);
            text1 = text1.Substring(num1);
            text2 = text2.Substring(num1);
            int num2 = this.diff_commonSuffix(text1, text2);
            string text4 = text1.Substring(text1.Length - num2);
            text1 = text1.Substring(0, text1.Length - num2);
            text2 = text2.Substring(0, text2.Length - num2);
            List<Diff> diffs = this.diff_compute(text1, text2, checklines, deadline);
            if (text3.Length != 0)
                diffs.Insert(0, new Diff(Operation.EQUAL, text3));
            if (text4.Length != 0)
                diffs.Add(new Diff(Operation.EQUAL, text4));
            this.diff_cleanupMerge(diffs);
            return diffs;
        }

        private List<Diff> diff_compute(
          string text1,
          string text2,
          bool checklines,
          DateTime deadline)
        {
            List<Diff> diffList1 = new List<Diff>();
            if (text1.Length == 0)
            {
                diffList1.Add(new Diff(Operation.INSERT, text2));
                return diffList1;
            }
            if (text2.Length == 0)
            {
                diffList1.Add(new Diff(Operation.DELETE, text1));
                return diffList1;
            }
            string str = text1.Length > text2.Length ? text1 : text2;
            string text3 = text1.Length > text2.Length ? text2 : text1;
            int length = str.IndexOf(text3, StringComparison.Ordinal);
            if (length != -1)
            {
                Operation operation = text1.Length > text2.Length ? Operation.DELETE : Operation.INSERT;
                diffList1.Add(new Diff(operation, str.Substring(0, length)));
                diffList1.Add(new Diff(Operation.EQUAL, text3));
                diffList1.Add(new Diff(operation, str.Substring(length + text3.Length)));
                return diffList1;
            }
            if (text3.Length == 1)
            {
                diffList1.Add(new Diff(Operation.DELETE, text1));
                diffList1.Add(new Diff(Operation.INSERT, text2));
                return diffList1;
            }
            string[] strArray = this.diff_halfMatch(text1, text2);
            if (strArray != null)
            {
                string text1_1 = strArray[0];
                string text1_2 = strArray[1];
                string text2_1 = strArray[2];
                string text2_2 = strArray[3];
                string text4 = strArray[4];
                List<Diff> diffList2 = this.diff_main(text1_1, text2_1, checklines, deadline);
                List<Diff> collection = this.diff_main(text1_2, text2_2, checklines, deadline);
                List<Diff> diffList3 = diffList2;
                diffList3.Add(new Diff(Operation.EQUAL, text4));
                diffList3.AddRange((IEnumerable<Diff>)collection);
                return diffList3;
            }
            return checklines && text1.Length > 100 && text2.Length > 100 ? this.diff_lineMode(text1, text2, deadline) : this.diff_bisect(text1, text2, deadline);
        }

        private List<Diff> diff_lineMode(string text1, string text2, DateTime deadline)
        {
            object[] chars = this.diff_linesToChars(text1, text2);
            text1 = (string)chars[0];
            text2 = (string)chars[1];
            List<string> lineArray = (List<string>)chars[2];
            List<Diff> diffs = this.diff_main(text1, text2, false, deadline);
            this.diff_charsToLines((ICollection<Diff>)diffs, lineArray);
            this.diff_cleanupSemantic(diffs);
            diffs.Add(new Diff(Operation.EQUAL, string.Empty));
            int index1 = 0;
            int num1 = 0;
            int num2 = 0;
            string empty1 = string.Empty;
            string empty2 = string.Empty;
            for (; index1 < diffs.Count; ++index1)
            {
                switch (diffs[index1].operation)
                {
                    case Operation.DELETE:
                        ++num1;
                        empty1 += diffs[index1].text;
                        break;
                    case Operation.INSERT:
                        ++num2;
                        empty2 += diffs[index1].text;
                        break;
                    case Operation.EQUAL:
                        if (num1 >= 1 && num2 >= 1)
                        {
                            diffs.RemoveRange(index1 - num1 - num2, num1 + num2);
                            int index2 = index1 - num1 - num2;
                            List<Diff> collection = this.diff_main(empty1, empty2, false, deadline);
                            diffs.InsertRange(index2, (IEnumerable<Diff>)collection);
                            index1 = index2 + collection.Count;
                        }
                        num2 = 0;
                        num1 = 0;
                        empty1 = string.Empty;
                        empty2 = string.Empty;
                        break;
                }
            }
            diffs.RemoveAt(diffs.Count - 1);
            return diffs;
        }

        protected List<Diff> diff_bisect(string text1, string text2, DateTime deadline)
        {
            int length1 = text1.Length;
            int length2 = text2.Length;
            int num1 = (length1 + length2 + 1) / 2;
            int num2 = num1;
            int length3 = 2 * num1;
            int[] numArray1 = new int[length3];
            int[] numArray2 = new int[length3];
            for (int index = 0; index < length3; ++index)
            {
                numArray1[index] = -1;
                numArray2[index] = -1;
            }
            numArray1[num2 + 1] = 0;
            numArray2[num2 + 1] = 0;
            int num3 = length1 - length2;
            bool flag = num3 % 2 != 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            int num7 = 0;
            for (int index1 = 0; index1 < num1 && !(DateTime.Now > deadline); ++index1)
            {
                for (int index2 = -index1 + num4; index2 <= index1 - num5; index2 += 2)
                {
                    int index3 = num2 + index2;
                    int num8 = index2 == -index1 || index2 != index1 && numArray1[index3 - 1] < numArray1[index3 + 1] ? numArray1[index3 + 1] : numArray1[index3 - 1] + 1;
                    int num9;
                    for (num9 = num8 - index2; num8 < length1 && num9 < length2 && (int)text1[num8] == (int)text2[num9]; ++num9)
                        ++num8;
                    numArray1[index3] = num8;
                    if (num8 > length1)
                        num5 += 2;
                    else if (num9 > length2)
                        num4 += 2;
                    else if (flag)
                    {
                        int index4 = num2 + num3 - index2;
                        if (index4 >= 0 && index4 < length3 && numArray2[index4] != -1)
                        {
                            int num10 = length1 - numArray2[index4];
                            if (num8 >= num10)
                                return this.diff_bisectSplit(text1, text2, num8, num9, deadline);
                        }
                    }
                }
                for (int index5 = -index1 + num6; index5 <= index1 - num7; index5 += 2)
                {
                    int index6 = num2 + index5;
                    int num11 = index5 == -index1 || index5 != index1 && numArray2[index6 - 1] < numArray2[index6 + 1] ? numArray2[index6 + 1] : numArray2[index6 - 1] + 1;
                    int num12;
                    for (num12 = num11 - index5; num11 < length1 && num12 < length2 && (int)text1[length1 - num11 - 1] == (int)text2[length2 - num12 - 1]; ++num12)
                        ++num11;
                    numArray2[index6] = num11;
                    if (num11 > length1)
                        num7 += 2;
                    else if (num12 > length2)
                        num6 += 2;
                    else if (!flag)
                    {
                        int index7 = num2 + num3 - index5;
                        if (index7 >= 0 && index7 < length3 && numArray1[index7] != -1)
                        {
                            int x = numArray1[index7];
                            int y = num2 + x - index7;
                            int num13 = length1 - numArray2[index6];
                            if (x >= num13)
                                return this.diff_bisectSplit(text1, text2, x, y, deadline);
                        }
                    }
                }
            }
            return new List<Diff>()
      {
        new Diff(Operation.DELETE, text1),
        new Diff(Operation.INSERT, text2)
      };
        }

        private List<Diff> diff_bisectSplit(
          string text1,
          string text2,
          int x,
          int y,
          DateTime deadline)
        {
            string text1_1 = text1.Substring(0, x);
            string text2_1 = text2.Substring(0, y);
            string text1_2 = text1.Substring(x);
            string text2_2 = text2.Substring(y);
            List<Diff> diffList = this.diff_main(text1_1, text2_1, false, deadline);
            diffList.AddRange((IEnumerable<Diff>)this.diff_main(text1_2, text2_2, false, deadline));
            return diffList;
        }

        protected object[] diff_linesToChars(string text1, string text2)
        {
            List<string> lineArray = new List<string>();
            Dictionary<string, int> lineHash = new Dictionary<string, int>();
            lineArray.Add(string.Empty);
            return new object[3]
            {
        (object) this.diff_linesToCharsMunge(text1, lineArray, lineHash),
        (object) this.diff_linesToCharsMunge(text2, lineArray, lineHash),
        (object) lineArray
            };
        }

        private string diff_linesToCharsMunge(
          string text,
          List<string> lineArray,
          Dictionary<string, int> lineHash)
        {
            int num1 = 0;
            int num2 = -1;
            StringBuilder stringBuilder = new StringBuilder();
            while (num2 < text.Length - 1)
            {
                num2 = text.IndexOf('\n', num1);
                if (num2 == -1)
                    num2 = text.Length - 1;
                string key = text.JavaSubstring(num1, num2 + 1);
                num1 = num2 + 1;
                if (lineHash.ContainsKey(key))
                {
                    stringBuilder.Append((char)lineHash[key]);
                }
                else
                {
                    lineArray.Add(key);
                    lineHash.Add(key, lineArray.Count - 1);
                    stringBuilder.Append((char)(lineArray.Count - 1));
                }
            }
            return stringBuilder.ToString();
        }

        protected void diff_charsToLines(ICollection<Diff> diffs, List<string> lineArray)
        {
            foreach (Diff diff in (IEnumerable<Diff>)diffs)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int index = 0; index < diff.text.Length; ++index)
                    stringBuilder.Append(lineArray[(int)diff.text[index]]);
                diff.text = stringBuilder.ToString();
            }
        }

        public int diff_commonPrefix(string text1, string text2)
        {
            int num = Math.Min(text1.Length, text2.Length);
            for (int index = 0; index < num; ++index)
            {
                if ((int)text1[index] != (int)text2[index])
                    return index;
            }
            return num;
        }

        public int diff_commonSuffix(string text1, string text2)
        {
            int length1 = text1.Length;
            int length2 = text2.Length;
            int num = Math.Min(text1.Length, text2.Length);
            for (int index = 1; index <= num; ++index)
            {
                if ((int)text1[length1 - index] != (int)text2[length2 - index])
                    return index - 1;
            }
            return num;
        }

        protected int diff_commonOverlap(string text1, string text2)
        {
            int length1 = text1.Length;
            int length2 = text2.Length;
            if (length1 == 0 || length2 == 0)
                return 0;
            if (length1 > length2)
                text1 = text1.Substring(length1 - length2);
            else if (length1 < length2)
                text2 = text2.Substring(0, length1);
            int num1 = Math.Min(length1, length2);
            if (text1 == text2)
                return num1;
            int num2 = 0;
            int length3 = 1;
            while (true)
            {
                int num3;
                do
                {
                    string str = text1.Substring(num1 - length3);
                    num3 = text2.IndexOf(str, StringComparison.Ordinal);
                    if (num3 == -1)
                        return num2;
                    length3 += num3;
                }
                while (num3 != 0 && !(text1.Substring(num1 - length3) == text2.Substring(0, length3)));
                num2 = length3;
                ++length3;
            }
        }

        protected string[] diff_halfMatch(string text1, string text2)
        {
            if ((double)this.Diff_Timeout <= 0.0)
                return (string[])null;
            string longtext = text1.Length > text2.Length ? text1 : text2;
            string shorttext = text1.Length > text2.Length ? text2 : text1;
            if (longtext.Length < 4 || shorttext.Length * 2 < longtext.Length)
                return (string[])null;
            string[] strArray1 = this.diff_halfMatchI(longtext, shorttext, (longtext.Length + 3) / 4);
            string[] strArray2 = this.diff_halfMatchI(longtext, shorttext, (longtext.Length + 1) / 2);
            if (strArray1 == null && strArray2 == null)
                return (string[])null;
            string[] strArray3 = strArray2 != null ? (strArray1 != null ? (strArray1[4].Length > strArray2[4].Length ? strArray1 : strArray2) : strArray2) : strArray1;
            if (text1.Length > text2.Length)
                return strArray3;
            return new string[5]
            {
        strArray3[2],
        strArray3[3],
        strArray3[0],
        strArray3[1],
        strArray3[4]
            };
        }

        private string[] diff_halfMatchI(string longtext, string shorttext, int i)
        {
            string str1 = longtext.Substring(i, longtext.Length / 4);
            int num = -1;
            string str2 = string.Empty;
            string str3 = string.Empty;
            string str4 = string.Empty;
            string str5 = string.Empty;
            string str6 = string.Empty;
            while (num < shorttext.Length && (num = shorttext.IndexOf(str1, num + 1, StringComparison.Ordinal)) != -1)
            {
                int length1 = this.diff_commonPrefix(longtext.Substring(i), shorttext.Substring(num));
                int length2 = this.diff_commonSuffix(longtext.Substring(0, i), shorttext.Substring(0, num));
                if (str2.Length < length2 + length1)
                {
                    str2 = shorttext.Substring(num - length2, length2) + shorttext.Substring(num, length1);
                    str3 = longtext.Substring(0, i - length2);
                    str4 = longtext.Substring(i + length1);
                    str5 = shorttext.Substring(0, num - length2);
                    str6 = shorttext.Substring(num + length1);
                }
            }
            if (str2.Length * 2 < longtext.Length)
                return (string[])null;
            return new string[5] { str3, str4, str5, str6, str2 };
        }

        public void diff_cleanupSemantic(List<Diff> diffs)
        {
            bool flag = false;
            Stack<int> intStack = new Stack<int>();
            string text1 = (string)null;
            int index1 = 0;
            int val1_1 = 0;
            int val2_1 = 0;
            int val1_2 = 0;
            int val2_2 = 0;
            for (; index1 < diffs.Count; ++index1)
            {
                if (diffs[index1].operation == Operation.EQUAL)
                {
                    intStack.Push(index1);
                    val1_1 = val1_2;
                    val2_1 = val2_2;
                    val1_2 = 0;
                    val2_2 = 0;
                    text1 = diffs[index1].text;
                }
                else
                {
                    if (diffs[index1].operation == Operation.INSERT)
                        val1_2 += diffs[index1].text.Length;
                    else
                        val2_2 += diffs[index1].text.Length;
                    if (text1 != null && text1.Length <= Math.Max(val1_1, val2_1) && text1.Length <= Math.Max(val1_2, val2_2))
                    {
                        diffs.Insert(intStack.Peek(), new Diff(Operation.DELETE, text1));
                        diffs[intStack.Peek() + 1].operation = Operation.INSERT;
                        intStack.Pop();
                        if (intStack.Count > 0)
                            intStack.Pop();
                        index1 = intStack.Count > 0 ? intStack.Peek() : -1;
                        val1_1 = 0;
                        val2_1 = 0;
                        val1_2 = 0;
                        val2_2 = 0;
                        text1 = (string)null;
                        flag = true;
                    }
                }
            }
            if (flag)
                this.diff_cleanupMerge(diffs);
            this.diff_cleanupSemanticLossless(diffs);
            for (int index2 = 1; index2 < diffs.Count; ++index2)
            {
                if (diffs[index2 - 1].operation == Operation.DELETE && diffs[index2].operation == Operation.INSERT)
                {
                    string text2 = diffs[index2 - 1].text;
                    string text3 = diffs[index2].text;
                    int num1 = this.diff_commonOverlap(text2, text3);
                    int num2 = this.diff_commonOverlap(text3, text2);
                    if (num1 >= num2)
                    {
                        if ((double)num1 >= (double)text2.Length / 2.0 || (double)num1 >= (double)text3.Length / 2.0)
                        {
                            diffs.Insert(index2, new Diff(Operation.EQUAL, text3.Substring(0, num1)));
                            diffs[index2 - 1].text = text2.Substring(0, text2.Length - num1);
                            diffs[index2 + 1].text = text3.Substring(num1);
                            ++index2;
                        }
                    }
                    else if ((double)num2 >= (double)text2.Length / 2.0 || (double)num2 >= (double)text3.Length / 2.0)
                    {
                        diffs.Insert(index2, new Diff(Operation.EQUAL, text2.Substring(0, num2)));
                        diffs[index2 - 1].operation = Operation.INSERT;
                        diffs[index2 - 1].text = text3.Substring(0, text3.Length - num2);
                        diffs[index2 + 1].operation = Operation.DELETE;
                        diffs[index2 + 1].text = text2.Substring(num2);
                        ++index2;
                    }
                    ++index2;
                }
            }
        }

        public void diff_cleanupSemanticLossless(List<Diff> diffs)
        {
            for (int index = 1; index < diffs.Count - 1; ++index)
            {
                if (diffs[index - 1].operation == Operation.EQUAL && diffs[index + 1].operation == Operation.EQUAL)
                {
                    string str1 = diffs[index - 1].text;
                    string str2 = diffs[index].text;
                    string two = diffs[index + 1].text;
                    int num1 = this.diff_commonSuffix(str1, str2);
                    if (num1 > 0)
                    {
                        string str3 = str2.Substring(str2.Length - num1);
                        str1 = str1.Substring(0, str1.Length - num1);
                        str2 = str3 + str2.Substring(0, str2.Length - num1);
                        two = str3 + two;
                    }
                    string str4 = str1;
                    string str5 = str2;
                    string str6 = two;
                    int num2 = this.diff_cleanupSemanticScore(str1, str2) + this.diff_cleanupSemanticScore(str2, two);
                    while (str2.Length != 0 && two.Length != 0 && (int)str2[0] == (int)two[0])
                    {
                        str1 += str2[0].ToString();
                        str2 = str2.Substring(1) + two[0].ToString();
                        two = two.Substring(1);
                        int num3 = this.diff_cleanupSemanticScore(str1, str2) + this.diff_cleanupSemanticScore(str2, two);
                        if (num3 >= num2)
                        {
                            num2 = num3;
                            str4 = str1;
                            str5 = str2;
                            str6 = two;
                        }
                    }
                    if (diffs[index - 1].text != str4)
                    {
                        if (str4.Length != 0)
                        {
                            diffs[index - 1].text = str4;
                        }
                        else
                        {
                            diffs.RemoveAt(index - 1);
                            --index;
                        }
                        diffs[index].text = str5;
                        if (str6.Length != 0)
                        {
                            diffs[index + 1].text = str6;
                        }
                        else
                        {
                            diffs.RemoveAt(index + 1);
                            --index;
                        }
                    }
                }
            }
        }

        private int diff_cleanupSemanticScore(string one, string two)
        {
            if (one.Length == 0 || two.Length == 0)
                return 6;
            char c1 = one[one.Length - 1];
            char c2 = two[0];
            bool flag1 = !char.IsLetterOrDigit(c1);
            bool flag2 = !char.IsLetterOrDigit(c2);
            bool flag3 = flag1 && char.IsWhiteSpace(c1);
            bool flag4 = flag2 && char.IsWhiteSpace(c2);
            bool flag5 = flag3 && char.IsControl(c1);
            bool flag6 = flag4 && char.IsControl(c2);
            if (((!flag5 ? 0 : (this.BLANKLINEEND.IsMatch(one) ? 1 : 0)) | (!flag6 ? (false ? 1 : 0) : (this.BLANKLINESTART.IsMatch(two) ? 1 : 0))) != 0)
                return 5;
            if (flag5 | flag6)
                return 4;
            if (((!flag1 ? 0 : (!flag3 ? 1 : 0)) & (flag4 ? 1 : 0)) != 0)
                return 3;
            if (flag3 | flag4)
                return 2;
            return flag1 | flag2 ? 1 : 0;
        }

        public void diff_cleanupEfficiency(List<Diff> diffs)
        {
            bool flag1 = false;
            Stack<int> intStack = new Stack<int>();
            string text = string.Empty;
            int index = 0;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            for (; index < diffs.Count; ++index)
            {
                if (diffs[index].operation == Operation.EQUAL)
                {
                    if (diffs[index].text.Length < (int)this.Diff_EditCost && flag4 | flag5)
                    {
                        intStack.Push(index);
                        flag2 = flag4;
                        flag3 = flag5;
                        text = diffs[index].text;
                    }
                    else
                    {
                        intStack.Clear();
                        text = string.Empty;
                    }
                    flag4 = flag5 = false;
                }
                else
                {
                    if (diffs[index].operation == Operation.DELETE)
                        flag5 = true;
                    else
                        flag4 = true;
                    if (text.Length != 0 && (flag2 & flag3 & flag4 & flag5 || text.Length < (int)this.Diff_EditCost / 2 && (flag2 ? 1 : 0) + (flag3 ? 1 : 0) + (flag4 ? 1 : 0) + (flag5 ? 1 : 0) == 3))
                    {
                        diffs.Insert(intStack.Peek(), new Diff(Operation.DELETE, text));
                        diffs[intStack.Peek() + 1].operation = Operation.INSERT;
                        intStack.Pop();
                        text = string.Empty;
                        if (flag2 & flag3)
                        {
                            flag4 = flag5 = true;
                            intStack.Clear();
                        }
                        else
                        {
                            if (intStack.Count > 0)
                                intStack.Pop();
                            index = intStack.Count > 0 ? intStack.Peek() : -1;
                            flag4 = flag5 = false;
                        }
                        flag1 = true;
                    }
                }
            }
            if (!flag1)
                return;
            this.diff_cleanupMerge(diffs);
        }

        public void diff_cleanupMerge(List<Diff> diffs)
        {
            diffs.Add(new Diff(Operation.EQUAL, string.Empty));
            int index1 = 0;
            int num1 = 0;
            int num2 = 0;
            string str1 = string.Empty;
            string str2 = string.Empty;
            while (index1 < diffs.Count)
            {
                switch (diffs[index1].operation)
                {
                    case Operation.DELETE:
                        ++num1;
                        str1 += diffs[index1].text;
                        ++index1;
                        continue;
                    case Operation.INSERT:
                        ++num2;
                        str2 += diffs[index1].text;
                        ++index1;
                        continue;
                    case Operation.EQUAL:
                        if (num1 + num2 > 1)
                        {
                            if (num1 != 0 && num2 != 0)
                            {
                                int num3 = this.diff_commonPrefix(str2, str1);
                                if (num3 != 0)
                                {
                                    if (index1 - num1 - num2 > 0 && diffs[index1 - num1 - num2 - 1].operation == Operation.EQUAL)
                                    {
                                        diffs[index1 - num1 - num2 - 1].text += str2.Substring(0, num3);
                                    }
                                    else
                                    {
                                        diffs.Insert(0, new Diff(Operation.EQUAL, str2.Substring(0, num3)));
                                        ++index1;
                                    }
                                    str2 = str2.Substring(num3);
                                    str1 = str1.Substring(num3);
                                }
                                int num4 = this.diff_commonSuffix(str2, str1);
                                if (num4 != 0)
                                {
                                    diffs[index1].text = str2.Substring(str2.Length - num4) + diffs[index1].text;
                                    str2 = str2.Substring(0, str2.Length - num4);
                                    str1 = str1.Substring(0, str1.Length - num4);
                                }
                            }
                            if (num1 == 0)
                                diffs.Splice<Diff>(index1 - num2, num1 + num2, new Diff(Operation.INSERT, str2));
                            else if (num2 == 0)
                                diffs.Splice<Diff>(index1 - num1, num1 + num2, new Diff(Operation.DELETE, str1));
                            else
                                diffs.Splice<Diff>(index1 - num1 - num2, num1 + num2, new Diff(Operation.DELETE, str1), new Diff(Operation.INSERT, str2));
                            index1 = index1 - num1 - num2 + (num1 != 0 ? 1 : 0) + (num2 != 0 ? 1 : 0) + 1;
                        }
                        else if (index1 != 0 && diffs[index1 - 1].operation == Operation.EQUAL)
                        {
                            diffs[index1 - 1].text += diffs[index1].text;
                            diffs.RemoveAt(index1);
                        }
                        else
                            ++index1;
                        num2 = 0;
                        num1 = 0;
                        str1 = string.Empty;
                        str2 = string.Empty;
                        continue;
                    default:
                        continue;
                }
            }
            if (diffs[diffs.Count - 1].text.Length == 0)
                diffs.RemoveAt(diffs.Count - 1);
            bool flag = false;
            for (int index2 = 1; index2 < diffs.Count - 1; ++index2)
            {
                if (diffs[index2 - 1].operation == Operation.EQUAL && diffs[index2 + 1].operation == Operation.EQUAL)
                {
                    if (diffs[index2].text.EndsWith(diffs[index2 - 1].text, StringComparison.Ordinal))
                    {
                        diffs[index2].text = diffs[index2 - 1].text + diffs[index2].text.Substring(0, diffs[index2].text.Length - diffs[index2 - 1].text.Length);
                        diffs[index2 + 1].text = diffs[index2 - 1].text + diffs[index2 + 1].text;
                        diffs.Splice<Diff>(index2 - 1, 1);
                        flag = true;
                    }
                    else if (diffs[index2].text.StartsWith(diffs[index2 + 1].text, StringComparison.Ordinal))
                    {
                        diffs[index2 - 1].text += diffs[index2 + 1].text;
                        diffs[index2].text = diffs[index2].text.Substring(diffs[index2 + 1].text.Length) + diffs[index2 + 1].text;
                        diffs.Splice<Diff>(index2 + 1, 1);
                        flag = true;
                    }
                }
            }
            if (!flag)
                return;
            this.diff_cleanupMerge(diffs);
        }

        public int diff_xIndex(List<Diff> diffs, int loc)
        {
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            Diff diff1 = (Diff)null;
            foreach (Diff diff2 in diffs)
            {
                if (diff2.operation != Operation.INSERT)
                    num1 += diff2.text.Length;
                if (diff2.operation != Operation.DELETE)
                    num2 += diff2.text.Length;
                if (num1 > loc)
                {
                    diff1 = diff2;
                    break;
                }
                num3 = num1;
                num4 = num2;
            }
            return diff1 != null && diff1.operation == Operation.DELETE ? num4 : num4 + (loc - num3);
        }

        public string diff_prettyHtml(List<Diff> diffs)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Diff diff in diffs)
            {
                string str = diff.text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "&para;<br>");
                switch (diff.operation)
                {
                    case Operation.DELETE:
                        stringBuilder.Append("<del style=\"background:#ffe6e6;\">").Append(str).Append("</del>");
                        continue;
                    case Operation.INSERT:
                        stringBuilder.Append("<ins style=\"background:#e6ffe6;\">").Append(str).Append("</ins>");
                        continue;
                    case Operation.EQUAL:
                        stringBuilder.Append("<span>").Append(str).Append("</span>");
                        continue;
                    default:
                        continue;
                }
            }
            return stringBuilder.ToString();
        }

        public string diff_text1(List<Diff> diffs)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Diff diff in diffs)
            {
                if (diff.operation != Operation.INSERT)
                    stringBuilder.Append(diff.text);
            }
            return stringBuilder.ToString();
        }

        public string diff_text2(List<Diff> diffs)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Diff diff in diffs)
            {
                if (diff.operation != Operation.DELETE)
                    stringBuilder.Append(diff.text);
            }
            return stringBuilder.ToString();
        }

        public int diff_levenshtein(List<Diff> diffs)
        {
            int num = 0;
            int val1 = 0;
            int val2 = 0;
            foreach (Diff diff in diffs)
            {
                switch (diff.operation)
                {
                    case Operation.DELETE:
                        val2 += diff.text.Length;
                        continue;
                    case Operation.INSERT:
                        val1 += diff.text.Length;
                        continue;
                    case Operation.EQUAL:
                        num += Math.Max(val1, val2);
                        val1 = 0;
                        val2 = 0;
                        continue;
                    default:
                        continue;
                }
            }
            return num + Math.Max(val1, val2);
        }

        public string diff_toDelta(List<Diff> diffs)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Diff diff in diffs)
            {
                switch (diff.operation)
                {
                    case Operation.DELETE:
                        stringBuilder.Append("-").Append(diff.text.Length).Append("\t");
                        continue;
                    case Operation.INSERT:
                        stringBuilder.Append("+").Append(diff_match_patch.UrlEncode(diff.text).Replace('+', ' ')).Append("\t");
                        continue;
                    case Operation.EQUAL:
                        stringBuilder.Append("=").Append(diff.text.Length).Append("\t");
                        continue;
                    default:
                        continue;
                }
            }
            string delta = stringBuilder.ToString();
            if (delta.Length != 0)
                delta = diff_match_patch.unescapeForEncodeUriCompatability(delta.Substring(0, delta.Length - 1));
            return delta;
        }

        public List<Diff> diff_fromDelta(string text1, string delta)
        {
            List<Diff> diffList = new List<Diff>();
            int startIndex = 0;
            string str1 = delta;
            string[] separator = new string[1] { "\t" };
            foreach (string str2 in str1.Split(separator, StringSplitOptions.None))
            {
                if (str2.Length != 0)
                {
                    string str3 = str2.Substring(1);
                    switch (str2[0])
                    {
                        case '+':
                            string text2 = diff_match_patch.UrlDecode(str3.Replace("+", "%2b"));
                            diffList.Add(new Diff(Operation.INSERT, text2));
                            continue;
                        case '-':
                        case '=':
                            int int32;
                            try
                            {
                                int32 = Convert.ToInt32(str3);
                            }
                            catch (FormatException ex)
                            {
                                throw new ArgumentException("Invalid number in diff_fromDelta: " + str3, (Exception)ex);
                            }
                            if (int32 < 0)
                                throw new ArgumentException("Negative number in diff_fromDelta: " + str3);
                            string text3;
                            try
                            {
                                text3 = text1.Substring(startIndex, int32);
                                startIndex += int32;
                            }
                            catch (ArgumentOutOfRangeException ex)
                            {
                                throw new ArgumentException("Delta length (" + startIndex.ToString() + ") larger than source text length (" + text1.Length.ToString() + ").", (Exception)ex);
                            }
                            if (str2[0] == '=')
                            {
                                diffList.Add(new Diff(Operation.EQUAL, text3));
                                continue;
                            }
                            diffList.Add(new Diff(Operation.DELETE, text3));
                            continue;
                        default:
                            throw new ArgumentException("Invalid diff operation in diff_fromDelta: " + str2[0].ToString());
                    }
                }
            }
            if (startIndex != text1.Length)
                throw new ArgumentException("Delta length (" + startIndex.ToString() + ") smaller than source text length (" + text1.Length.ToString() + ").");
            return diffList;
        }

        public int match_main(string text, string pattern, int loc)
        {
            loc = Math.Max(0, Math.Min(loc, text.Length));
            if (text == pattern)
                return 0;
            if (text.Length == 0)
                return -1;
            return loc + pattern.Length <= text.Length && text.Substring(loc, pattern.Length) == pattern ? loc : this.match_bitap(text, pattern, loc);
        }

        protected int match_bitap(string text, string pattern, int loc)
        {
            Dictionary<char, int> dictionary = this.match_alphabet(pattern);
            double val2 = (double)this.Match_Threshold;
            int x1 = text.IndexOf(pattern, loc, StringComparison.Ordinal);
            if (x1 != -1)
            {
                val2 = Math.Min(this.match_bitapScore(0, x1, loc, pattern), val2);
                int x2 = text.LastIndexOf(pattern, Math.Min(loc + pattern.Length, text.Length), StringComparison.Ordinal);
                if (x2 != -1)
                    val2 = Math.Min(this.match_bitapScore(0, x2, loc, pattern), val2);
            }
            int num1 = 1 << pattern.Length - 1;
            int num2 = -1;
            int num3 = pattern.Length + text.Length;
            int[] numArray1 = new int[0];
            for (int e = 0; e < pattern.Length; ++e)
            {
                int num4 = 0;
                int num5;
                for (num5 = num3; num4 < num5; num5 = (num3 - num4) / 2 + num4)
                {
                    if (this.match_bitapScore(e, loc + num5, loc, pattern) <= val2)
                        num4 = num5;
                    else
                        num3 = num5;
                }
                num3 = num5;
                int num6 = Math.Max(1, loc - num5 + 1);
                int num7 = Math.Min(loc + num5, text.Length) + pattern.Length;
                int[] numArray2 = new int[num7 + 2];
                numArray2[num7 + 1] = (1 << e) - 1;
                for (int index = num7; index >= num6; --index)
                {
                    int num8 = text.Length <= index - 1 || !dictionary.ContainsKey(text[index - 1]) ? 0 : dictionary[text[index - 1]];
                    numArray2[index] = e != 0 ? (numArray2[index + 1] << 1 | 1) & num8 | (numArray1[index + 1] | numArray1[index]) << 1 | 1 | numArray1[index + 1] : (numArray2[index + 1] << 1 | 1) & num8;
                    if ((numArray2[index] & num1) != 0)
                    {
                        double num9 = this.match_bitapScore(e, index - 1, loc, pattern);
                        if (num9 <= val2)
                        {
                            val2 = num9;
                            num2 = index - 1;
                            if (num2 > loc)
                                num6 = Math.Max(1, 2 * loc - num2);
                            else
                                break;
                        }
                    }
                }
                if (this.match_bitapScore(e + 1, loc, loc, pattern) <= val2)
                    numArray1 = numArray2;
                else
                    break;
            }
            return num2;
        }

        private double match_bitapScore(int e, int x, int loc, string pattern)
        {
            float num1 = (float)e / (float)pattern.Length;
            int num2 = Math.Abs(loc - x);
            if (this.Match_Distance != 0)
                return (double)num1 + (double)num2 / (double)this.Match_Distance;
            return num2 != 0 ? 1.0 : (double)num1;
        }

        protected Dictionary<char, int> match_alphabet(string pattern)
        {
            Dictionary<char, int> dictionary = new Dictionary<char, int>();
            char[] charArray = pattern.ToCharArray();
            foreach (char key in charArray)
            {
                if (!dictionary.ContainsKey(key))
                    dictionary.Add(key, 0);
            }
            int num1 = 0;
            foreach (char key in charArray)
            {
                int num2 = dictionary[key] | 1 << pattern.Length - num1 - 1;
                dictionary[key] = num2;
                ++num1;
            }
            return dictionary;
        }

        protected void patch_addContext(Patch patch, string text)
        {
            if (text.Length == 0)
                return;
            string str = text.Substring(patch.start2, patch.length1);
            int num1 = 0;
            for (; text.IndexOf(str, StringComparison.Ordinal) != text.LastIndexOf(str, StringComparison.Ordinal) && str.Length < (int)this.Match_MaxBits - (int)this.Patch_Margin - (int)this.Patch_Margin; str = text.JavaSubstring(Math.Max(0, patch.start2 - num1), Math.Min(text.Length, patch.start2 + patch.length1 + num1)))
                num1 += (int)this.Patch_Margin;
            int num2 = num1 + (int)this.Patch_Margin;
            string text1 = text.JavaSubstring(Math.Max(0, patch.start2 - num2), patch.start2);
            if (text1.Length != 0)
                patch.diffs.Insert(0, new Diff(Operation.EQUAL, text1));
            string text2 = text.JavaSubstring(patch.start2 + patch.length1, Math.Min(text.Length, patch.start2 + patch.length1 + num2));
            if (text2.Length != 0)
                patch.diffs.Add(new Diff(Operation.EQUAL, text2));
            patch.start1 -= text1.Length;
            patch.start2 -= text1.Length;
            patch.length1 += text1.Length + text2.Length;
            patch.length2 += text1.Length + text2.Length;
        }

        public List<Patch> patch_make(string text1, string text2)
        {
            List<Diff> diffs = this.diff_main(text1, text2, true);
            if (diffs.Count > 2)
            {
                this.diff_cleanupSemantic(diffs);
                this.diff_cleanupEfficiency(diffs);
            }
            return this.patch_make(text1, diffs);
        }

        public List<Patch> patch_make(List<Diff> diffs) => this.patch_make(this.diff_text1(diffs), diffs);

        public List<Patch> patch_make(string text1, string text2, List<Diff> diffs) => this.patch_make(text1, diffs);

        public List<Patch> patch_make(string text1, List<Diff> diffs)
        {
            List<Patch> patchList = new List<Patch>();
            if (diffs.Count == 0)
                return patchList;
            Patch patch = new Patch();
            int num = 0;
            int startIndex = 0;
            string text = text1;
            string str = text1;
            foreach (Diff diff in diffs)
            {
                if (patch.diffs.Count == 0 && diff.operation != Operation.EQUAL)
                {
                    patch.start1 = num;
                    patch.start2 = startIndex;
                }
                switch (diff.operation)
                {
                    case Operation.DELETE:
                        patch.length1 += diff.text.Length;
                        patch.diffs.Add(diff);
                        str = str.Remove(startIndex, diff.text.Length);
                        break;
                    case Operation.INSERT:
                        patch.diffs.Add(diff);
                        patch.length2 += diff.text.Length;
                        str = str.Insert(startIndex, diff.text);
                        break;
                    case Operation.EQUAL:
                        if (diff.text.Length <= 2 * (int)this.Patch_Margin && patch.diffs.Count<Diff>() != 0 && diff != diffs.Last<Diff>())
                        {
                            patch.diffs.Add(diff);
                            patch.length1 += diff.text.Length;
                            patch.length2 += diff.text.Length;
                        }
                        if (diff.text.Length >= 2 * (int)this.Patch_Margin && patch.diffs.Count != 0)
                        {
                            this.patch_addContext(patch, text);
                            patchList.Add(patch);
                            patch = new Patch();
                            text = str;
                            num = startIndex;
                            break;
                        }
                        break;
                }
                if (diff.operation != Operation.INSERT)
                    num += diff.text.Length;
                if (diff.operation != Operation.DELETE)
                    startIndex += diff.text.Length;
            }
            if (patch.diffs.Count != 0)
            {
                this.patch_addContext(patch, text);
                patchList.Add(patch);
            }
            return patchList;
        }

        public List<Patch> patch_deepCopy(List<Patch> patches)
        {
            List<Patch> patchList = new List<Patch>();
            foreach (Patch patch1 in patches)
            {
                Patch patch2 = new Patch();
                foreach (Diff diff1 in patch1.diffs)
                {
                    Diff diff2 = new Diff(diff1.operation, diff1.text);
                    patch2.diffs.Add(diff2);
                }
                patch2.start1 = patch1.start1;
                patch2.start2 = patch1.start2;
                patch2.length1 = patch1.length1;
                patch2.length2 = patch1.length2;
                patchList.Add(patch2);
            }
            return patchList;
        }

        public object[] patch_apply(List<Patch> patches, string text)
        {
            if (patches.Count == 0)
                return new object[2]
                {
          (object) text,
          (object) new bool[0]
                };
            patches = this.patch_deepCopy(patches);
            string str1 = this.patch_addPadding(patches);
            text = str1 + text + str1;
            this.patch_splitMax(patches);
            int index = 0;
            int num1 = 0;
            bool[] flagArray = new bool[patches.Count];
            foreach (Patch patch in patches)
            {
                int loc1 = patch.start2 + num1;
                string str2 = this.diff_text1(patch.diffs);
                int num2 = -1;
                int num3;
                if (str2.Length > (int)this.Match_MaxBits)
                {
                    num3 = this.match_main(text, str2.Substring(0, (int)this.Match_MaxBits), loc1);
                    if (num3 != -1)
                    {
                        num2 = this.match_main(text, str2.Substring(str2.Length - (int)this.Match_MaxBits), loc1 + str2.Length - (int)this.Match_MaxBits);
                        if (num2 == -1 || num3 >= num2)
                            num3 = -1;
                    }
                }
                else
                    num3 = this.match_main(text, str2, loc1);
                if (num3 == -1)
                {
                    flagArray[index] = false;
                    num1 -= patch.length2 - patch.length1;
                }
                else
                {
                    flagArray[index] = true;
                    num1 = num3 - loc1;
                    string text2 = num2 != -1 ? text.JavaSubstring(num3, Math.Min(num2 + (int)this.Match_MaxBits, text.Length)) : text.JavaSubstring(num3, Math.Min(num3 + str2.Length, text.Length));
                    if (str2 == text2)
                    {
                        text = text.Substring(0, num3) + this.diff_text2(patch.diffs) + text.Substring(num3 + str2.Length);
                    }
                    else
                    {
                        List<Diff> diffs = this.diff_main(str2, text2, false);
                        if (str2.Length > (int)this.Match_MaxBits && (double)this.diff_levenshtein(diffs) / (double)str2.Length > (double)this.Patch_DeleteThreshold)
                        {
                            flagArray[index] = false;
                        }
                        else
                        {
                            this.diff_cleanupSemanticLossless(diffs);
                            int loc2 = 0;
                            foreach (Diff diff in patch.diffs)
                            {
                                if (diff.operation != Operation.EQUAL)
                                {
                                    int num4 = this.diff_xIndex(diffs, loc2);
                                    if (diff.operation == Operation.INSERT)
                                        text = text.Insert(num3 + num4, diff.text);
                                    else if (diff.operation == Operation.DELETE)
                                        text = text.Remove(num3 + num4, this.diff_xIndex(diffs, loc2 + diff.text.Length) - num4);
                                }
                                if (diff.operation != Operation.DELETE)
                                    loc2 += diff.text.Length;
                            }
                        }
                    }
                }
                ++index;
            }
            text = text.Substring(str1.Length, text.Length - 2 * str1.Length);
            return new object[2]
            {
        (object) text,
        (object) flagArray
            };
        }

        public string patch_addPadding(List<Patch> patches)
        {
            short patchMargin = this.Patch_Margin;
            string empty = string.Empty;
            for (short index = 1; (int)index <= (int)patchMargin; ++index)
                empty += ((char)index).ToString();
            foreach (Patch patch in patches)
            {
                patch.start1 += (int)patchMargin;
                patch.start2 += (int)patchMargin;
            }
            Patch patch1 = patches.First<Patch>();
            List<Diff> diffs1 = patch1.diffs;
            if (diffs1.Count == 0 || diffs1.First<Diff>().operation != Operation.EQUAL)
            {
                diffs1.Insert(0, new Diff(Operation.EQUAL, empty));
                patch1.start1 -= (int)patchMargin;
                patch1.start2 -= (int)patchMargin;
                patch1.length1 += (int)patchMargin;
                patch1.length2 += (int)patchMargin;
            }
            else if ((int)patchMargin > diffs1.First<Diff>().text.Length)
            {
                Diff diff = diffs1.First<Diff>();
                int num = (int)patchMargin - diff.text.Length;
                diff.text = empty.Substring(diff.text.Length) + diff.text;
                patch1.start1 -= num;
                patch1.start2 -= num;
                patch1.length1 += num;
                patch1.length2 += num;
            }
            Patch patch2 = patches.Last<Patch>();
            List<Diff> diffs2 = patch2.diffs;
            if (diffs2.Count == 0 || diffs2.Last<Diff>().operation != Operation.EQUAL)
            {
                diffs2.Add(new Diff(Operation.EQUAL, empty));
                patch2.length1 += (int)patchMargin;
                patch2.length2 += (int)patchMargin;
            }
            else if ((int)patchMargin > diffs2.Last<Diff>().text.Length)
            {
                Diff diff = diffs2.Last<Diff>();
                int length = (int)patchMargin - diff.text.Length;
                diff.text += empty.Substring(0, length);
                patch2.length1 += length;
                patch2.length2 += length;
            }
            return empty;
        }

        public void patch_splitMax(List<Patch> patches)
        {
            short matchMaxBits = this.Match_MaxBits;
            for (int index = 0; index < patches.Count; ++index)
            {
                if (patches[index].length1 > (int)matchMaxBits)
                {
                    Patch patch1 = patches[index];
                    patches.Splice<Patch>(index--, 1);
                    int start1 = patch1.start1;
                    int start2 = patch1.start2;
                    string text1 = string.Empty;
                    while (patch1.diffs.Count != 0)
                    {
                        Patch patch2 = new Patch();
                        bool flag = true;
                        patch2.start1 = start1 - text1.Length;
                        patch2.start2 = start2 - text1.Length;
                        if (text1.Length != 0)
                        {
                            patch2.length1 = patch2.length2 = text1.Length;
                            patch2.diffs.Add(new Diff(Operation.EQUAL, text1));
                        }
                        while (patch1.diffs.Count != 0 && patch2.length1 < (int)matchMaxBits - (int)this.Patch_Margin)
                        {
                            Operation operation = patch1.diffs[0].operation;
                            string text2 = patch1.diffs[0].text;
                            switch (operation)
                            {
                                case Operation.DELETE:
                                    if (patch2.diffs.Count == 1 && patch2.diffs.First<Diff>().operation == Operation.EQUAL && text2.Length > 2 * (int)matchMaxBits)
                                    {
                                        patch2.length1 += text2.Length;
                                        start1 += text2.Length;
                                        flag = false;
                                        patch2.diffs.Add(new Diff(operation, text2));
                                        patch1.diffs.RemoveAt(0);
                                        continue;
                                    }
                                    break;
                                case Operation.INSERT:
                                    patch2.length2 += text2.Length;
                                    start2 += text2.Length;
                                    patch2.diffs.Add(patch1.diffs.First<Diff>());
                                    patch1.diffs.RemoveAt(0);
                                    flag = false;
                                    continue;
                            }
                            string text3 = text2.Substring(0, Math.Min(text2.Length, (int)matchMaxBits - patch2.length1 - (int)this.Patch_Margin));
                            patch2.length1 += text3.Length;
                            start1 += text3.Length;
                            if (operation == Operation.EQUAL)
                            {
                                patch2.length2 += text3.Length;
                                start2 += text3.Length;
                            }
                            else
                                flag = false;
                            patch2.diffs.Add(new Diff(operation, text3));
                            if (text3 == patch1.diffs[0].text)
                                patch1.diffs.RemoveAt(0);
                            else
                                patch1.diffs[0].text = patch1.diffs[0].text.Substring(text3.Length);
                        }
                        string str = this.diff_text2(patch2.diffs);
                        text1 = str.Substring(Math.Max(0, str.Length - (int)this.Patch_Margin));
                        string text4 = this.diff_text1(patch1.diffs).Length <= (int)this.Patch_Margin ? this.diff_text1(patch1.diffs) : this.diff_text1(patch1.diffs).Substring(0, (int)this.Patch_Margin);
                        if (text4.Length != 0)
                        {
                            patch2.length1 += text4.Length;
                            patch2.length2 += text4.Length;
                            if (patch2.diffs.Count != 0 && patch2.diffs[patch2.diffs.Count - 1].operation == Operation.EQUAL)
                                patch2.diffs[patch2.diffs.Count - 1].text += text4;
                            else
                                patch2.diffs.Add(new Diff(Operation.EQUAL, text4));
                        }
                        if (!flag)
                            patches.Splice<Patch>(++index, 0, patch2);
                    }
                }
            }
        }

        public string patch_toText(List<Patch> patches)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Patch patch in patches)
                stringBuilder.Append((object)patch);
            return stringBuilder.ToString();
        }

        public List<Patch> patch_fromText(string textline)
        {
            List<Patch> patchList = new List<Patch>();
            if (textline.Length == 0)
                return patchList;
            string[] strArray = textline.Split('\n');
            int index = 0;
            Regex regex = new Regex("^@@ -(\\d+),?(\\d*) \\+(\\d+),?(\\d*) @@$");
        label_25:
            while (index < strArray.Length)
            {
                Match match = regex.Match(strArray[index]);
                if (!match.Success)
                    throw new ArgumentException("Invalid patch string: " + strArray[index]);
                Patch patch = new Patch();
                patchList.Add(patch);
                patch.start1 = Convert.ToInt32(match.Groups[1].Value);
                if (match.Groups[2].Length == 0)
                {
                    --patch.start1;
                    patch.length1 = 1;
                }
                else if (match.Groups[2].Value == "0")
                {
                    patch.length1 = 0;
                }
                else
                {
                    --patch.start1;
                    patch.length1 = Convert.ToInt32(match.Groups[2].Value);
                }
                patch.start2 = Convert.ToInt32(match.Groups[3].Value);
                if (match.Groups[4].Length == 0)
                {
                    --patch.start2;
                    patch.length2 = 1;
                }
                else if (match.Groups[4].Value == "0")
                {
                    patch.length2 = 0;
                }
                else
                {
                    --patch.start2;
                    patch.length2 = Convert.ToInt32(match.Groups[4].Value);
                }
                ++index;
                while (index < strArray.Length)
                {
                    char ch;
                    try
                    {
                        ch = strArray[index][0];
                    }
                    catch (IndexOutOfRangeException ex)
                    {
                        ++index;
                        continue;
                    }
                    string text = diff_match_patch.UrlDecode(strArray[index].Substring(1).Replace("+", "%2b"));
                    switch (ch)
                    {
                        case ' ':
                            patch.diffs.Add(new Diff(Operation.EQUAL, text));
                            break;
                        case '+':
                            patch.diffs.Add(new Diff(Operation.INSERT, text));
                            break;
                        case '-':
                            patch.diffs.Add(new Diff(Operation.DELETE, text));
                            break;
                        case '@':
                            goto label_25;
                        default:
                            throw new ArgumentException("Invalid patch mode '" + ch.ToString() + "' in: " + text);
                    }
                    ++index;
                }
            }
            return patchList;
        }

        public static string unescapeForEncodeUriCompatability(string str) => str.Replace("%21", "!").Replace("%7e", "~").Replace("%27", "'").Replace("%28", "(").Replace("%29", ")").Replace("%3b", ";").Replace("%2f", "/").Replace("%3f", "?").Replace("%3a", ":").Replace("%40", "@").Replace("%26", "&").Replace("%3d", "=").Replace("%2b", "+").Replace("%24", "$").Replace("%2c", ",").Replace("%23", "#");

        internal static string UrlEncode(string str)
        {
            str = WebUtility.UrlEncode(str);
            return Regex.Replace(str, "(%[0-9A-F]{2})", (MatchEvaluator)(encodedChar => encodedChar.Value.ToLowerInvariant()));
        }

        internal static string UrlDecode(string str) => WebUtility.UrlDecode(str);
    }

    public class Patch
    {
        public List<Diff> diffs = new List<Diff>();
        public int start1;
        public int start2;
        public int length1;
        public int length2;

        public override string ToString()
        {
            string str1 = this.length1 != 0 ? (this.length1 != 1 ? (this.start1 + 1).ToString() + "," + this.length1.ToString() : Convert.ToString(this.start1 + 1)) : this.start1.ToString() + ",0";
            string str2 = this.length2 != 0 ? (this.length2 != 1 ? (this.start2 + 1).ToString() + "," + this.length2.ToString() : Convert.ToString(this.start2 + 1)) : this.start2.ToString() + ",0";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("@@ -").Append(str1).Append(" +").Append(str2).Append(" @@\n");
            foreach (Diff diff in this.diffs)
            {
                switch (diff.operation)
                {
                    case Operation.DELETE:
                        stringBuilder.Append('-');
                        break;
                    case Operation.INSERT:
                        stringBuilder.Append('+');
                        break;
                    case Operation.EQUAL:
                        stringBuilder.Append(' ');
                        break;
                }
                stringBuilder.Append(diff_match_patch.UrlEncode(diff.text).Replace('+', ' ')).Append("\n");
            }
            return diff_match_patch.unescapeForEncodeUriCompatability(stringBuilder.ToString());
        }
    }

    public class Diff
    {
        public Operation operation;
        public string text;

        public Diff(Operation operation, string text)
        {
            this.operation = operation;
            this.text = text;
        }

        public override string ToString()
        {
            string str = this.text.Replace('\n', '¶');
            return "Diff(" + this.operation.ToString() + ",\"" + str + "\")";
        }

        public override bool Equals(object obj) => obj != null && obj is Diff diff && diff.operation == this.operation && diff.text == this.text;

        public bool Equals(Diff obj) => obj != null && obj.operation == this.operation && obj.text == this.text;

        public override int GetHashCode() => this.text.GetHashCode() ^ this.operation.GetHashCode();
    }
    public enum Operation
    {
        DELETE,
        INSERT,
        EQUAL,
    }

    internal class Lcs
    {
        internal List<JToken> Sequence { get; set; }

        internal List<int> Indices1 { get; set; }

        internal List<int> Indices2 { get; set; }

        private Lcs()
        {
            this.Sequence = new List<JToken>();
            this.Indices1 = new List<int>();
            this.Indices2 = new List<int>();
        }

        internal static Lcs Get(List<JToken> left, List<JToken> right) => Lcs.Backtrack(Lcs.LcsInternal(left, right), left, right, left.Count, right.Count);

        private static int[,] LcsInternal(List<JToken> left, List<JToken> right)
        {
            int[,] numArray = new int[left.Count + 1, right.Count + 1];
            for (int index1 = 1; index1 <= left.Count; ++index1)
            {
                for (int index2 = 1; index2 <= right.Count; ++index2)
                    numArray[index1, index2] = !JToken.DeepEquals(left[index1 - 1], right[index2 - 1]) ? Math.Max(numArray[index1 - 1, index2], numArray[index1, index2 - 1]) : numArray[index1 - 1, index2 - 1] + 1;
            }
            return numArray;
        }

        private static Lcs Backtrack(
            int[,] matrix,
            List<JToken> left,
            List<JToken> right,
            int li,
            int ri)
        {
            Lcs lcs = new Lcs();
            int index1 = 1;
            int index2 = 1;
            while (index1 <= li && index2 <= ri)
            {
                if (JToken.DeepEquals(left[index1 - 1], right[index2 - 1]) || left[index1 - 1].Type == JTokenType.Object && right[index2 - 1].Type == JTokenType.Object || left[index1 - 1].Type == JTokenType.Array && right[index2 - 1].Type == JTokenType.Array)
                {
                    lcs.Sequence.Add(left[index1 - 1]);
                    lcs.Indices1.Add(index1 - 1);
                    lcs.Indices2.Add(index2 - 1);
                    ++index1;
                    ++index2;
                }
                else if (matrix[index1, index2 - 1] > matrix[index1 - 1, index2])
                    ++index1;
                else
                    ++index2;
            }
            return lcs;
        }
    }

    internal static class CompatibilityExtensions
    {
        // JScript splice function
        public static List<T> Splice<T>(this List<T> input, int start, int count,
            params T[] objects)
        {
            List<T> deletedRange = input.GetRange(start, count);
            input.RemoveRange(start, count);
            input.InsertRange(start, objects);

            return deletedRange;
        }

        // Java substring function
        public static string JavaSubstring(this string s, int begin, int end)
        {
            return s.Substring(begin, end - begin);
        }
    }
}
