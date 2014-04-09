using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GLGUI.Advanced
{
    public class GLDataControl : GLScrollableControl
    {
        public GLSkin.GLLabelSkin LabelSkin { get { return labelSkin; } set { labelSkin = value; if(history.Count > 0) SetData(history.First()); } }
        public GLSkin.GLLabelSkin LinkLabelSkin { get { return linkLabelSkin; } set { linkLabelSkin = value; if (history.Count > 0) SetData(history.First()); } }

        public Dictionary<Type, string[]> Hidden = new Dictionary<Type, string[]>();
        public Dictionary<Type, Tuple<string, Func<object>>[]> Links = new Dictionary<Type, Tuple<string, Func<object>>[]>();
        public Dictionary<Type, Func<object, object>> Shortcuts = new Dictionary<Type, Func<object, object>>();

        private int row;
        private Stack<Func<object>> history = new Stack<Func<object>>();
        private List<Tuple<Func<object>, GLLabel>> updateData = new List<Tuple<Func<object>, GLLabel>>(256);

        private GLFlowLayout horizontal, left, right;
        private GLSkin.GLLabelSkin labelSkin, linkLabelSkin;

        public GLDataControl(GLGui gui) : base(gui)
        {
            Render += (s, d) => UpdateData();

            horizontal = Add(new GLFlowLayout(gui) { FlowDirection = GLFlowDirection.LeftToRight, AutoSize = true });
            left = horizontal.Add(new GLFlowLayout(gui) { FlowDirection = GLFlowDirection.TopDown, AutoSize = true });
            var skin = left.Skin;
            skin.Space = 0;
            left.Skin = skin;
            right = horizontal.Add(new GLFlowLayout(gui) { FlowDirection = GLFlowDirection.TopDown, AutoSize = true });
            right.Skin = skin;

			labelSkin = gui.Skin.LabelEnabled;
            linkLabelSkin = gui.Skin.LinkLabelEnabled;

			// defaults
			Hidden.Add(typeof(IEnumerable), new string[] {
				"_items", "_size", "_version", "_syncRoot",
				"m_buckets", "m_slots", "m_count", "m_lastIndex", "m_freeList", "m_comparer",
				"m_version", "m_siInfo", "m_collection", "m_boundedCapacity", "m_freeNodes",
				"m_occupiedNodes", "m_isDisposed", "m_ConsumersCancellationTokenSource",
				"m_ProducersCancellationTokenSource", "m_currentAdders",
				"buckets", "entries", "count", "version", "freeList", "freeCount", "comparer", "keys", "values",
				"IsFixedSize", "IsReadOnly", "IsSynchronized", "SyncRoot" });
			Hidden.Add(typeof(Array), new string[] { "LongLength", "Rank", "Count" });
			Hidden.Add(typeof(KeyValuePair<,>), new string[] { "key", "value" });
			Hidden.Add(typeof(Dictionary<,>), new string[] { "Keys", "Values" });
        }

        private void AddRow(string name, EventHandler click, Func<object> value)
        {
			var link = left.Add(new GLLinkLabel(Gui) { AutoSize = true, Text = name, SkinEnabled = linkLabelSkin });
            if (value() != null)
				link.Click += click;
			var label = right.Add(new GLLabel(Gui) { AutoSize = true, Text = (value() ?? "null").ToString(), SkinEnabled = labelSkin });
            updateData.Add(new Tuple<Func<object>, GLLabel>(value, label));
            row++;
        }

        public void SetData(object obj)
        {
            setData(() => obj, true);
        }

        private List<T> getRelated<T>(Dictionary<Type, T[]> dict, Type type)
        {
            List<T> list = new List<T>();

            while (type != null)
            {
                if (dict.ContainsKey(type))
                    list.AddRange(dict[type]);

                if (type.IsGenericType)
                {
                    Type genType = type.GetGenericTypeDefinition();
                    if (dict.ContainsKey(genType))
                        list.AddRange(dict[genType]);
                }

                var interfaces = type.GetInterfaces();
                foreach(var iface in interfaces)
                {
                    if (dict.ContainsKey(iface))
                        list.AddRange(dict[iface]);

                    if (iface.IsGenericType)
                    {
                        Type ifaceGenType = iface.GetGenericTypeDefinition();
                        if (dict.ContainsKey(ifaceGenType))
                            list.AddRange(dict[ifaceGenType]);
                    }
                }

                type = type.BaseType;
            }

            return list;
        }

        private void considerShortcutsAndAdd(string name, Func<object> obj)
        {
            Func<object> value = obj;
            var o = obj();
            if (o != null)
            {
                Type type = o.GetType();
                Func<object, object> shortcut;

                while (type != null)
                {
                    if (Shortcuts.TryGetValue(type, out shortcut))
                    {
                        value = () => shortcut(obj());
                        break;
                    }
                    if (type.IsGenericType)
                    {
                        Type genType = type.GetGenericTypeDefinition();
                        if (Shortcuts.TryGetValue(genType, out shortcut))
                        {
                            value = () => shortcut(obj());
                            break;
                        }
                    }
                    var interfaces = type.GetInterfaces();
                    foreach (var iface in interfaces)
                    {
                        if (Shortcuts.TryGetValue(iface, out shortcut))
                        {
                            value = () => shortcut(obj());
                            goto shortcutFound;
                        }
                        if (iface.IsGenericType)
                        {
                            Type ifaceGenType = iface.GetGenericTypeDefinition();
                            if (Shortcuts.TryGetValue(ifaceGenType, out shortcut))
                            {
                                value = () => shortcut(obj());
                                goto shortcutFound;
                            }
                        }
                    }
                    type = type.BaseType;
                }
            }
            
            shortcutFound:
            AddRow(name, (s, e) => setData(value, false), value);
        }

        private void setData(Func<object> obj, bool resetHistory)
        {
            left.Clear();
            right.Clear();
            Horizontal.Value = 0.0f;
            Vertical.Value = 0.0f;
            updateData.Clear();

            if (resetHistory)
                history.Clear();

            row = 0;

            if (history.Count > 0)
            {
                AddRow("[..]", (s, e) =>
                {
                    if (history.Count < 2)
                    {
                        setData(null, true);
                        return;
                    }
                    history.Pop();
                    setData(history.Pop(), false);
                }, history.Peek());
            }

            if (obj == null || obj() == null)
                return;

            try
            {
                Type type = obj().GetType();
                Type rtype = type.BaseType;

                var hidden = getRelated<string>(Hidden, type);
                var links = getRelated<Tuple<string, Func<object>>>(Links, type);

                // fields
                rtype = type.BaseType;
                var fields = new Dictionary<string, FieldInfo>();
                foreach (var f in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    string name = f.Name.Split('.').Last();
                    if (!fields.ContainsKey(name))
                        fields.Add(name, f);
                }
                while (rtype != null)
                {
                    var addFields = rtype.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                    foreach (var af in addFields)
                    {
                        string name = af.Name.Split('.').Last();
                        if (!fields.ContainsKey(name))
                            fields.Add(name, af);
                    }
                    rtype = rtype.BaseType;
                }
                foreach (var field in fields)
                {
                    if (field.Key[0] == '<') // skip property backing fields
                        continue;
                    if (hidden.Contains(field.Key))
                        continue;
                    var f = field.Value;
                    considerShortcutsAndAdd(field.Key, () => { try { return f.GetValue(obj()); } catch (Exception) { return null; } });
                }

                // properties
                rtype = type.BaseType;
                var properties = new Dictionary<string, PropertyInfo>();
                foreach (var p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    string name = p.Name.Split('.').Last();
                    if (!properties.ContainsKey(name))
                        properties.Add(name, p);
                }
                while (rtype != null)
                {
                    var addProperties = rtype.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
                    foreach (var ap in addProperties)
                    {
                        string name = ap.Name.Split('.').Last();
                        if (!properties.ContainsKey(name))
                            properties.Add(name, ap);
                    }
                    rtype = rtype.BaseType;
                }
                foreach (var property in properties)
                {
                    if (hidden.Contains(property.Key))
                        continue;
                    var p = property.Value;
                    try { p.GetValue(obj(), null); }
                    catch (Exception) { continue; }
                    considerShortcutsAndAdd(property.Key, () => { try { return p.GetValue(obj(), null); } catch (Exception) { return "Exception caught in get method"; } });
                }

                // links
                foreach (var link in links)
                {
                    Func<object> linkObj = link.Item2;
                    considerShortcutsAndAdd(link.Item1, () => { try { return linkObj; } catch (Exception) { return "Exception caught in link method"; } });
                }

                // arrays and collections
                if (type.IsArray)
                {
                    Array array = (Array)obj();
                    for (int i = 0; i < array.Length; i++)
                    {
                        int j = i;
                        considerShortcutsAndAdd("[" + i + "]", () => { try { return ((Array)obj()).GetValue(j); } catch (Exception) { return null; } });
                    }
                }
                else if (type.GetInterfaces().Contains(typeof(ICollection)))
                {
                    int i = 0;
                    try
                    {
                        foreach (object item in ((IEnumerable)obj()))
                        {
                            int j = i++;
                            considerShortcutsAndAdd("[" + i + "]", () => { try { return ((IEnumerable)obj()).Cast<object>().ElementAt(j); } catch(Exception) { return null; } });
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        AddRow("retry", (s, e) => { setData(history.Pop(), false); }, () => "InvalidOperationException");
                    }
                    catch (IndexOutOfRangeException)
                    {
                        AddRow("retry", (s, e) => { setData(history.Pop(), false); }, () => "IndexOutOfRangeException");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[GLDataControl] Unknown error occured:\n{0}", e.ToString());
                SetData(null);
            }

            history.Push(obj);
        }

        MethodInfo updateLayout = typeof(GLLabel).GetMethod("UpdateLayout", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic);
        object[] updateParams = new object[0];

        public void UpdateData()
        {
            Gui.SuspendLayout();
            bool changed = false;
            foreach(var u in updateData)
            {
                try
                {
                    string text = (u.Item1() ?? "null").ToString();
                    if (u.Item2.Text != text)
                    {
                        u.Item2.Text = text;
                        updateLayout.Invoke(u.Item2, updateParams);
                        changed = true;
                    }
                }
                catch(Exception)
                {
                    if (history.Count > 1)
                    {
                        history.Pop();
                        setData(history.Pop(), false);
                    }
                    else
                    {
                        setData(null, true);
                    }
                    Gui.ResumeLayout();
                    return;
                }
            }
            Gui.ResumeLayout();
            if(changed)
                right.Invalidate();
        }
    }
}
