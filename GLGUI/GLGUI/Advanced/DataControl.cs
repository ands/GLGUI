using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using GLGUI;

namespace GLGUI.Advanced
{
    public partial class DataControl : GLScrollableControl
    {
        public Dictionary<Type, string[]> Hidden = new Dictionary<Type, string[]>();
        public Dictionary<Type, Tuple<string, Func<object>>[]> Links = new Dictionary<Type, Tuple<string, Func<object>>[]>();
        public Dictionary<Type, Func<object, object>> Shortcuts = new Dictionary<Type, Func<object, object>>();

        private int row;
        private Stack<Func<object>> history = new Stack<Func<object>>();
        private List<Tuple<Func<object>, GLLabel>> updateData = new List<Tuple<Func<object>, GLLabel>>(256);

        private GLFlowLayout horizontal, left, right;

        public DataControl(GLGui gui) : base(gui)
        {
            Render += (s, e) => UpdateData();

            horizontal = Add(new GLFlowLayout(gui) { FlowDirection = FlowDirection.LeftToRight, AutoSize = true });
            left = horizontal.Add(new GLFlowLayout(gui) { FlowDirection = FlowDirection.TopDown, AutoSize = true });
            var skin = left.Skin;
            skin.Space = 0;
            left.Skin = skin;
            right = horizontal.Add(new GLFlowLayout(gui) { FlowDirection = FlowDirection.TopDown, AutoSize = true });
            right.Skin = skin;
        }

        private void AddRow(string name, EventHandler click, Func<object> value)
        {
			var link = left.Add(new GLLinkLabel(Gui) { AutoSize = true, Text = name });
            if (value() != null)
				link.Click += click;
			var label = right.Add(new GLLabel(Gui) { AutoSize = true, Text = (value() ?? "null").ToString() });
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
            while(rtype != null)
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
                considerShortcutsAndAdd(field.Key, () => f.GetValue(obj()));
            }

            // properties
            rtype = type.BaseType;
            var properties = new Dictionary<string, PropertyInfo>();
            foreach(var p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
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
                    considerShortcutsAndAdd("[" + i + "]", () => ((Array)obj()).GetValue(j));
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
                        considerShortcutsAndAdd("[" + i + "]", () => ((IEnumerable)obj()).Cast<object>().ElementAt(j));
                    }
                }
                catch(InvalidOperationException)
                {
                    AddRow("retry", (s, e) => { setData(history.Pop(), false); }, () => "InvalidOperationException");
                }
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
