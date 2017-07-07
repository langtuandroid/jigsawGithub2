using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public static class JsonFormatter
{
	private const string INDENT_STRING = "    ";
    public static string FormatJson(string str)
    {
		// Trim off any trailing new line characters
		str = str.TrimEnd('\n');
		
		// Based on answer from http://stackoverflow.com/questions/4580397/json-formatter-in-c
		// Extended with the really bad assumption that strings containing a new line character are already well formatted
		if(!str.Contains("\n"))
		{
	        var indent = 0;
	        var quoted = false;
	        var sb = new StringBuilder();
	        for (var i = 0; i < str.Length; i++)
	        {
	            var ch = str[i];
	            switch (ch)
	            {
	                case '{':
	                case '[':
	                    sb.Append(ch);
	                    if (!quoted)
	                    {
	                        sb.AppendLine();
							++indent;
							for (int j=0; j<indent; j++)
								sb.Append(INDENT_STRING);
	                        //Enumerable.Range(0, ++indent).ForEach(item => sb.Append(INDENT_STRING));
	                    }
	                    break;
	                case '}':
	                case ']':
	                    if (!quoted)
	                    {
	                        sb.AppendLine();
							--indent;
							for (int j=0; j<indent; j++)
								sb.Append(INDENT_STRING);
	                        //Enumerable.Range(0, --indent).ForEach(item => sb.Append(INDENT_STRING));
	                    }
	                    sb.Append(ch);
	                    break;
	                case '"':
	                    sb.Append(ch);
	                    bool escaped = false;
	                    var index = i;
	                    while (index > 0 && str[--index] == '\\')
	                        escaped = !escaped;
	                    if (!escaped)
	                        quoted = !quoted;
	                    break;
	                case ',':
	                    sb.Append(ch);
	                    if (!quoted)
	                    {
	                        sb.AppendLine();
							for (int j=0; j<indent; j++)
								sb.Append(INDENT_STRING);
	                        //Enumerable.Range(0, indent).ForEach(item => sb.Append(INDENT_STRING));
	                    }
	                    break;
	                case ':':
	                    sb.Append(ch);
	                    if (!quoted)
	                        sb.Append(" ");
	                    break;
	                default:
	                    sb.Append(ch);
	                    break;
	            }
	        }
	        return sb.ToString();
		}
		else
		{
			return str;
		}
    }
}
