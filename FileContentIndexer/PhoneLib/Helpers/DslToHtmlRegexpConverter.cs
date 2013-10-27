using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PhoneLib.Helpers
{
	public static class DslToHtmlRegexpConverter
	{
		private class RegexPair
		{
			public Regex RegExp { get; set; }
			public string Replacement { get; set; }
		}

		public static string ConvertStringToHtml(string stringToConvert)
		{
			var regexes = new List<RegexPair>() 
			{
				new RegexPair()
				{
					RegExp = new Regex("(\\[m[1-9]{0,2}\\])"),
					Replacement = "<div class='paragraph'>"
				},
				new RegexPair()
				{
					RegExp = new Regex("(\\[/m\\])"),
					Replacement = "</div>"
				},
				new RegexPair()
				{
					RegExp = new Regex("(\\[trn\\])"),
					Replacement = "<div class='translation'>"
				},
				new RegexPair()
				{
					RegExp = new Regex("(\\[/trn\\])"),
					Replacement = "</div>"
				},
				new RegexPair()
				{
					RegExp = new Regex("(\\[lang[\\sname=\"[a-zA-Z]{0,}\"]{0,}\\])"),
					Replacement = "<span class='lang'>"
				},
				new RegexPair()
				{
					RegExp = new Regex("(\\[/lang[.]*\\])"),
					Replacement = "</span>"
				},
				new RegexPair()
				{
					RegExp = new Regex("(\\[com\\])"),
					Replacement = "<span class='com'>"
				},
				new RegexPair()
				{
					RegExp = new Regex("(\\[i\\])"),
					Replacement = "<span class='italics'>"
				},
				new RegexPair()
				{
					RegExp = new Regex("(\\[c\\])"),
					Replacement = "<span class='coloured'>"
				},
				new RegexPair()
				{
					RegExp = new Regex("(\\[p\\])"),
					Replacement = "<span class='p'>"
				},
				new RegexPair()
				{
					RegExp = new Regex("(\\[b\\])"),
					Replacement = "<span class='bold'>"
				},
				new RegexPair()
				{
					RegExp = new Regex("(\\[u\\])"),
					Replacement = "<span class='underline'>"
				},
				new RegexPair()
				{
					RegExp = new Regex("(\\[\\*\\])"),
					Replacement = "<span class='fullTrn'>"
				},
				new RegexPair()
				{
					RegExp = new Regex("(\\[/((com)|(i)|(c)|(p)|(b)|(u)|(\\*))\\])"),
					Replacement = "</span>"
				},
				new RegexPair()
				{
					RegExp = new Regex(@"\\\["),
					Replacement = "["
				},
				new RegexPair()
				{
					RegExp = new Regex(@"\\\]"),
					Replacement = "]"
				},
				new RegexPair()
				{
					RegExp = new Regex("(((\\[)|(\\[/))[a-zA-Z*]{1,}\\])"),
					Replacement = string.Empty
				}
			};

			var resultString = stringToConvert;

			foreach (var pair in regexes)
			{
				resultString = pair.RegExp.Replace(resultString, pair.Replacement);
			}

			return resultString;
		}
	}
}
