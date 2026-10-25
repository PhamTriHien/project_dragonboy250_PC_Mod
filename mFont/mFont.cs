using System;
using System.Collections;
using UnityEngine;

public partial class mFont
{
	public static int LEFT = 0;

	public static int RIGHT = 1;

	public static int CENTER = 2;

	public static int RED = 0;

	public static int YELLOW = 1;

	public static int GREEN = 2;

	public static int FATAL = 3;

	public static int MISS = 4;

	public static int ORANGE = 5;

	public static int ADDMONEY = 6;

	public static int MISS_ME = 7;

	public static int FATAL_ME = 8;

	public static int HP = 9;

	public static int MP = 10;

	private int space;

	private Image imgFont;

	private string strFont;

	private int[][] fImages;

	public static int yAddFont;

	public static int[] colorJava = new int[31]
		{
			0, 16711680, 6520319, 16777215, 16755200, 5449989, 21285, 52224, 7386228, 16771788,
			0, 65535, 21285, 16776960, 5592405, 16742263, 33023, 8701737, 15723503, 7999781,
			16768815, 14961237, 4124899, 4671303, 16096312, 16711680, 16755200, 52224, 16777215, 6520319,
			16096312
		};

	public static mFont gI;

	public static mFont tahoma_7b_red;

	public static mFont tahoma_7b_blue;

	public static mFont tahoma_7b_white;

	public static mFont tahoma_7b_yellow;

	public static mFont tahoma_7b_yellowSmall;

	public static mFont tahoma_7b_dark;

	public static mFont tahoma_7b_green2;

	public static mFont tahoma_7b_green;

	public static mFont tahoma_7b_focus;

	public static mFont tahoma_7b_unfocus;

	public static mFont tahoma_7;

	public static mFont tahoma_7_blue1;

	public static mFont tahoma_7_blue1Small;

	public static mFont tahoma_7_green2;

	public static mFont tahoma_7_yellow;

	public static mFont tahoma_7_grey;

	public static mFont tahoma_7_red;

	public static mFont tahoma_7_blue;

	public static mFont tahoma_7_green;

	public static mFont tahoma_7_white;

	public static mFont tahoma_8b;

	public static mFont number_yellow;

	public static mFont number_red;

	public static mFont number_green;

	public static mFont number_gray;

	public static mFont number_orange;

	public static mFont bigNumber_red;

	public static mFont bigNumber_While;

	public static mFont bigNumber_yellow;

	public static mFont bigNumber_green;

	public static mFont bigNumber_orange;

	public static mFont bigNumber_blue;

	public static mFont bigNumber_black;

	public static mFont nameFontRed;

	public static mFont nameFontYellow;

	public static mFont nameFontGreen;

	public static mFont tahoma_7_greySmall;

	public static mFont tahoma_7b_yellowSmall2;

	public static mFont tahoma_7b_green2Small;

	public static mFont tahoma_7_whiteSmall;

	public static mFont tahoma_7b_greenSmall;

	public Font myFont;

	private int height;

	private int wO;

	public Color color1 = Color.white;

	public Color color2 = Color.gray;

	public sbyte id;

	public int fstyle;

	public string st1 = "áàảãạăắằẳẵặâấầẩẫậéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵđÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴĐ";

	public string st2 = "\u00b8µ¶·¹\u00a8¾»¼½Æ©ÊÇÈÉËÐÌÎÏÑªÕÒÓÔÖÝ×ØÜÞãßáâä«èåæçé¬íêëìîóïñòô\u00adøõö÷ùýúûüþ®\u00b8µ¶·¹¡¾»¼½Æ¢ÊÇÈÉËÐÌÎÏÑ£ÕÒÓÔÖÝ×ØÜÞãßáâä¤èåæçé¥íêëìîóïñòô¦øõö÷ùýúûüþ§";

	public const string str = " 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW";

	private int yAdd;

	private string pathImage;

	private string pathData;

	private int javaHeight;

	private const string JavaDataTahoma7Base64 = "AOAAAAAAAAMAAQAAAAEABQALAAAADAAEAAsAAAAXAAUACwAAACIABQALAAAALQAGAAsAAAA4AAUACwAAAEMABQALAAAATgAFAAsAAABZAAUACwAAAGQABQALAAAAbwAGAAsAAAB6AAMACwAAAIUABgALAAAAkAAHAAsAAACbAAIACwAAAKYABgALAAAAsQAFAAsAAAC8AAIACwAAAMcAAwALAAAA0gAGAAsAAADdAAYACwAAAOgABAALAAAA8wADAAsAAAD+AAMACwAAAQkABAALAAABFAAEAAsAAAEfAAIACwAAASoACAALAAABNQAIAAsAAAFAAAYACwAAAUsACgALAAABVgAGAAsAAAFhAAcACwAAAWwABgALAAABdwAEAAsAAAGCAAQACwAAAY0AAgALAAABmAAFAAsAAAGjAAUACwAAAa4ABQALAAABuQAFAAsAAAHEAAUACwAAAc8ABQALAAAB2gAFAAsAAAHlAAYACwAAAfAABQALAAAB+wAHAAsAAAIGAAUACwAAAhEABQALAAACHAAFAAsAAAInAAUACwAAAjIABQALAAACPQAFAAsAAAJIAAUACwAAAlMABQALAAACXgAFAAsAAAJpAAQACwAAAnQABQALAAACfwAGAAsAAAKKAAUACwAAApUABQALAAACoAAFAAsAAAKrAAUACwAAArYABQALAAACwQAFAAsAAALMAAUACwAAAtcABgALAAAC4gAFAAsAAALtAAYACwAAAvgABQALAAADAwAFAAsAAAMOAAQACwAAAxkABQALAAADJAAFAAsAAAMvAAIACwAAAzoAAwALAAADRQADAAsAAANQAAMACwAAA1sABQALAAADZgACAAsAAANxAAMACwAAA3wABQALAAADhwACAAsAAAOSAAgACwAAA50ABQALAAADqAAFAAsAAAOzAAUACwAAA74ABQALAAADyQAFAAsAAAPUAAUACwAAA98ABQALAAAD6gAFAAsAAAP1AAUACwAABAAABQALAAAECwAGAAsAAAQWAAUACwAABCEABQALAAAELAAGAAsAAAQ3AAYACwAABEIABgALAAAETQAGAAsAAARYAAYACwAABGMABgALAAAEbgAFAAsAAAR5AAUACwAABIQABAALAAAEjwAEAAsAAASaAAMACwAABKUABQALAAAEsAAFAAsAAAS7AAUACwAABMYABQALAAAE0QAFAAsAAATcAAUACwAABOcABgALAAAE8gAGAAsAAAT9AAYACwAABQgABgALAAAFEwAGAAsAAAUeAAYACwAABSkABgALAAAFNAAEAAsAAAU/AAYACwAABUoABgALAAAFVQAGAAsAAAVgAAYACwAABWsABgALAAAFdgAGAAsAAAWBAAQACwAABYwACAALAAAFlwAHAAsAAAWiAAcACwAABa0ABwALAAAFuAAHAAsAAAXDAAcACwAABc4ABwALAAAF2QAHAAsAAAXkAAcACwAABe8ABwALAAAF+gAHAAsAAAYFAAcACwAABhAABwALAAAGGwAHAAsAAAYmAAcACwAABjEABwALAAAGPAAHAAsAAAZHAAcACwAABlIABwALAAAGXQAGAAsAAAZoAAcACwAABnMABwALAAAGfgAHAAsAAAaJAAYACwAABpQABgALAAAGnwAGAAsAAAaqAAYACwAABrUABgALAAAGwAAGAAsAAAbLAAYACwAABtYABgALAAAG4QAGAAsAAAbsAAcACwAABvcABgALAAAHAgAGAAsAAAcNAAYACwAABxgABwALAAAHIwAHAAsAAAcuAAQACwAABzkABAALAAAHRAAEAAsAAAdPAAQACwAAB1oABAALAAAHZQAEAAsAAAdwAAQACwAAB3sABgALAAAHhgAFAAsAAAeRAAgACwAAB5wABwALAAAHpwAIAAsAAAeyAAgACwAAB70ACAALAAAHyAAIAAsAAAfTAAgACwAAB94ACAALAAAH6QAIAAsAAAf0AAgACwAAB/8ACAALAAAICgAIAAsAAAgVAAgACwAACCAACAALAAAIKwAJAAsAAAg2AAkACwAACEEACQALAAAITAAJAAsAAAhXAAkACwAACGIACQALAAAIbQAGAAsAAAh4AAgACwAACIMABgALAAAIjgAGAAsAAAiZAAYACwAACKQABwALAAAIrwAHAAsAAAi6AAcACwAACMUABwALAAAI0AAHAAsAAAjbAAcACwAACOYACAALAAAI8QAIAAsAAAj8AAgACwAACQcACAALAAAJEgAIAAsAAAkdAAgACwAACSgABwALAAAJMwAFAAsAAAk+AAYACwAACUkABgALAAAJVAAGAAsAAAlfAAYACwAACWoABgALAAAJdQAGAAsAAAmAAAUACwAACYsACAAL";

	private const string JavaDataTahoma7bBase64 = "AOAAAAAAAAMAAQAAAAEABwALAAAADAAFAAsAAAAXAAcACwAAACIABwALAAAALQAHAAsAAAA4AAcACwAAAEMABwALAAAATgAHAAsAAABZAAcACwAAAGQABwALAAAAbwAGAAsAAAB6AAQACwAAAIUABgALAAAAkAAGAAsAAACbAAIACwAAAKYABwALAAAAsQAGAAsAAAC8AAMACwAAAMcAAgALAAAA0gAHAAsAAADdAAcACwAAAOgABQALAAAA8wAFAAsAAAD+AAUACwAAAQkABwALAAABFAAHAAsAAAEfAAMACwAAASoACgALAAABNQAHAAsAAAFAAAcACwAAAUsADAALAAABVgAHAAsAAAFhAAgACwAAAWwABgALAAABdwAFAAsAAAGCAAUACwAAAY0AAwALAAABmAAGAAsAAAGjAAYACwAAAa4ABgALAAABuQAGAAsAAAHEAAYACwAAAc8ABgALAAAB2gAGAAsAAAHlAAcACwAAAfAABgALAAAB+wAIAAsAAAIGAAYACwAAAhEABgALAAACHAAGAAsAAAInAAYACwAAAjIABgALAAACPQAGAAsAAAJIAAYACwAAAlMABgALAAACXgAGAAsAAAJpAAUACwAAAnQABgALAAACfwAHAAsAAAKKAAYACwAAApUABgALAAACoAAGAAsAAAKrAAYACwAAArYABgALAAACwQAGAAsAAALMAAYACwAAAtcABwALAAAC4gAGAAsAAALtAAcACwAAAvgABgALAAADAwAGAAsAAAMOAAUACwAAAxkABgALAAADJAAGAAsAAAMvAAMACwAAAzoAAwALAAADRQADAAsAAANQAAMACwAAA1sABQALAAADZgADAAsAAANxAAQACwAAA3wABgALAAADhwADAAsAAAOSAAkACwAAA50ABgALAAADqAAGAAsAAAOzAAYACwAAA74ABgALAAADyQAGAAsAAAPUAAYACwAAA98ABgALAAAD6gAGAAsAAAP1AAYACwAABAAABgALAAAECwAHAAsAAAQWAAYACwAABCEABgALAAAELAAHAAsAAAQ3AAcACwAABEIABwALAAAETQAHAAsAAARYAAcACwAABGMABwALAAAEbgAGAAsAAAR5AAYACwAABIQABQALAAAEjwAFAAsAAASaAAQACwAABKUABgALAAAEsAAGAAsAAAS7AAYACwAABMYABgALAAAE0QAGAAsAAATcAAYACwAABOcACAALAAAE8gAIAAsAAAT9AAgACwAABQgACAALAAAFEwAIAAsAAAUeAAgACwAABSkABgALAAAFNAAGAAsAAAU/AAYACwAABUoABgALAAAFVQAGAAsAAAVgAAYACwAABWsABgALAAAFdgAHAAsAAAWBAAUACwAABYwACAALAAAFlwAIAAsAAAWiAAgACwAABa0ACAALAAAFuAAIAAsAAAXDAAgACwAABc4ACAALAAAF2QAIAAsAAAXkAAgACwAABe8ACAALAAAF+gAIAAsAAAYFAAgACwAABhAACAALAAAGGwAIAAsAAAYmAAgACwAABjEACAALAAAGPAAJAAsAAAZHAAgACwAABlIACAALAAAGXQAHAAsAAAZoAAcACwAABnMABwALAAAGfgAHAAsAAAaJAAYACwAABpQABgALAAAGnwAGAAsAAAaqAAYACwAABrUABgALAAAGwAAGAAsAAAbLAAYACwAABtYABgALAAAG4QAGAAsAAAbsAAgACwAABvcABgALAAAHAgAGAAsAAAcNAAYACwAABxgABwALAAAHIwAIAAsAAAcuAAUACwAABzkABQALAAAHRAAFAAsAAAdPAAUACwAAB1oABQALAAAHZQAFAAsAAAdwAAUACwAAB3sABwALAAAHhgAGAAsAAAeRAAkACwAAB5wABwALAAAHpwAIAAsAAAeyAAgACwAAB70ACAALAAAHyAAIAAsAAAfTAAgACwAAB94ACAALAAAH6QAIAAsAAAf0AAgACwAAB/8ACAALAAAICgAJAAsAAAgVAAgACwAACCAACAALAAAIKwAKAAsAAAg2AAoACwAACEEACgALAAAITAAKAAsAAAhXAAoACwAACGIACgALAAAIbQAHAAsAAAh4AAgACwAACIMABwALAAAIjgAGAAsAAAiZAAcACwAACKQABwALAAAIrwAHAAsAAAi6AAcACwAACMUABwALAAAI0AAHAAsAAAjbAAcACwAACOYACQALAAAI8QAJAAsAAAj8AAkACwAACQcACQALAAAJEgAJAAsAAAkdAAkACwAACSgABwALAAAJMwAHAAsAAAk+AAcACwAACUkABwALAAAJVAAHAAsAAAlfAAcACwAACWoABwALAAAJdQAHAAsAAAmAAAYACwAACYsACQAL";

	private const string JavaDataTahoma8bBase64 = "AOAAAAAAAAMAAQAAAAEACAAOAAAADwAGAA4AAAAdAAgADgAAACsACAAOAAAAOQAIAA4AAABHAAgADgAAAFUACAAOAAAAYwAIAA4AAABxAAgADgAAAH8ACAAOAAAAjQAJAA4AAACbAAYADgAAAKkACAAOAAAAtwAJAA4AAADFAAQADgAAANMACQAOAAAA4QAHAA4AAADvAAQADgAAAP0ABAAOAAABCwAJAA4AAAEZAAkADgAAAScABwAOAAABNQAGAA4AAAFDAAYADgAAAVEACAAOAAABXwAIAA4AAAFtAAQADgAAAXsACwAOAAABiQAKAA4AAAGXAAgADgAAAaUADgAOAAABswAJAA4AAAHBAAoADgAAAc8ACAAOAAAB3QAGAA4AAAHrAAYADgAAAfkABAAOAAACBwAIAA4AAAIVAAgADgAAAiMACAAOAAACMQAIAA4AAAI/AAgADgAAAk0ACAAOAAACWwAIAA4AAAJpAAkADgAAAncACAAOAAAChQAKAA4AAAKTAAgADgAAAqEACAAOAAACrwAIAA4AAAK9AAgADgAAAssACAAOAAAC2QAIAA4AAALnAAgADgAAAvUACAAOAAADAwAIAA4AAAMRAAcADgAAAx8ACAAOAAADLQAJAA4AAAM7AAgADgAAA0kACAAOAAADVwAIAA4AAANlAAgADgAAA3MACAAOAAADgQAIAA4AAAOPAAgADgAAA50ACQAOAAADqwAIAA4AAAO5AAkADgAAA8cACAAOAAAD1QAIAA4AAAPjAAYADgAAA/EACAAOAAAD/wAIAA4AAAQNAAQADgAABBsABQAOAAAEKQAFAA4AAAQ3AAQADgAABEUABgAOAAAEUwAEAA4AAARhAAUADgAABG8ACAAOAAAEfQAEAA4AAASLAAwADgAABJkACAAOAAAEpwAIAA4AAAS1AAgADgAABMMACAAOAAAE0QAIAA4AAATfAAgADgAABO0ACAAOAAAE+wAIAA4AAAUJAAgADgAABRcACAAOAAAFJQAJAA4AAAUzAAgADgAABUEACAAOAAAFTwAJAA4AAAVdAAkADgAABWsACQAOAAAFeQAJAA4AAAWHAAkADgAABZUACQAOAAAFowAIAA4AAAWxAAgADgAABb8ABgAOAAAFzQAHAA4AAAXbAAYADgAABekACAAOAAAF9wAIAA4AAAYFAAgADgAABhMACAAOAAAGIQAIAA4AAAYvAAgADgAABj0ACgAOAAAGSwAKAA4AAAZZAAoADgAABmcACgAOAAAGdQAKAA4AAAaDAAoADgAABpEACAAOAAAGnwAIAA4AAAatAAgADgAABrsACAAOAAAGyQAIAA4AAAbXAAgADgAABuUACAAOAAAG8wAIAA4AAAcBAAcADgAABw8ACgAOAAAHHQAJAA4AAAcrAAkADgAABzkACQAOAAAHRwAJAA4AAAdVAAkADgAAB2MACQAOAAAHcQAJAA4AAAd/AAkADgAAB40ACQAOAAAHmwAJAA4AAAepAAkADgAAB7cACQAOAAAHxQAJAA4AAAfTAAkADgAAB+EACQAOAAAH7wALAA4AAAf9AAkADgAACAsACQAOAAAIGQAIAA4AAAgnAAgADgAACDUACQAOAAAIQwAKAA4AAAhRAAcADgAACF8ABwAOAAAIbQAHAA4AAAh7AAcADgAACIkABwAOAAAIlwAHAA4AAAilAAcADgAACLMACAAOAAAIwQAIAA4AAAjPAAoADgAACN0ABwAOAAAI6wAHAA4AAAj5AAcADgAACQcACQAOAAAJFQAJAA4AAAkjAAYADgAACTEABgAOAAAJPwAGAA4AAAlNAAYADgAACVsABwAOAAAJaQAGAA4AAAl3AAcADgAACYUACAAOAAAJkwAHAA4AAAmhAAsADgAACa8ACAAOAAAJvQAJAA4AAAnLAAkADgAACdkACQAOAAAJ5wAJAA4AAAn1AAkADgAACgMACQAOAAAKEQAJAA4AAAofAAkADgAACi0ACQAOAAAKOwALAA4AAApJAAkADgAAClcACQAOAAAKZQALAA4AAApzAAsADgAACoEACwAOAAAKjwALAA4AAAqdAAsADgAACqsACwAOAAAKuQAIAA4AAArHAAkADgAACtUACQAOAAAK4wAIAA4AAArxAAgADgAACv8ACQAOAAALDQAJAA4AAAsbAAkADgAACykACQAOAAALNwAJAA4AAAtFAAkADgAAC1MACwAOAAALYQALAA4AAAtvAAsADgAAC30ACwAOAAALiwALAA4AAAuZAAsADgAAC6cACAAOAAALtQAIAA4AAAvDAAgADgAAC9EACAAOAAAL3wAIAA4AAAvtAAgADgAAC/sACAAOAAAMCQAIAA4AAAwXAAgADgAADCUADAAO";

	private const string JavaDataNumberBase64 = "AA0AAAAAAAEAAQAAAAEABQAIAAAACQADAAgAAAARAAUACAAAABkABQAIAAAAIQAFAAgAAAApAAUACAAAADEABQAIAAAAOQAFAAgAAABBAAUACAAAAEkABQAIAAAAUQAFAAgAAABZAAUACA==";

	private static readonly System.Collections.Generic.Dictionary<string, int[][]> s_CachedGlyphTables = new System.Collections.Generic.Dictionary<string, int[][]>(8);
	private static readonly System.Collections.Generic.Dictionary<string, int> s_CachedGlyphHeights = new System.Collections.Generic.Dictionary<string, int>(8);

	public static sbyte cl__white;

	public static sbyte cl__yellow;

	public static sbyte cl__yellowSmall;

	public static sbyte cl_dark = 0;

	public static sbyte cl_green = 1;

	public static sbyte cl__blue = 2;

	public static sbyte cl_red = 3;

	public mFont(string strFont, string pathImage, string pathData, int space)
		{
			try
			{
				this.strFont = strFont;
				this.space = space;
				this.pathImage = pathImage;
				this.pathData = pathData;
				LoadJavaGlyphMetrics(pathData);
				reloadImage();
				if (javaHeight > 0)
				{
					setHeight(javaHeight);
				}
			}
			catch (Exception ex3)
			{
				ex3.StackTrace.ToString();
			}
		}

	public mFont(sbyte id)
		{
			string text = "chelthm";
			if ((id > 0 && id < 10) || id == 19)
			{
				yAdd = 1;
				text = "barmeneb";
			}
			else if (id >= 10 && id <= 18)
			{
				text = "chelthm";
				yAdd = 2;
			}
			else if (id > 24)
			{
				text = "staccato";
			}
			this.id = id;
			text = "FontSys/x" + mGraphics.zoomLevel + "/" + text;
			myFont = (Font)Resources.Load(text);
			if (id < 25)
			{
				color1 = setColorFont(id);
				color2 = setColorFont(id);
			}
			else
			{
				color1 = bigColor(id);
				color2 = bigColor(id);
			}
			wO = getWidthExactOf("o");
			InitJavaBitmapDataById(id);
			s_AllInstances.Add(this);
		}

	private void InitJavaBitmapDataById(sbyte fontId)
	{
		const string numberChars = " 0123456789+-";
		space = 0;
		switch (fontId)
		{
		case 1:
			strFont = str;
			pathImage = "/myfont/tahoma_7b_red.png";
			pathData = "/myfont/tahoma_7b";
			break;
		case 2:
			strFont = str;
			pathImage = "/myfont/tahoma_7b_blue.png";
			pathData = "/myfont/tahoma_7b";
			break;
		case 3:
		case 28:
			strFont = str;
			pathImage = "/myfont/tahoma_7b_white.png";
			pathData = "/myfont/tahoma_7b";
			break;
		case 4:
			strFont = str;
			pathImage = "/myfont/tahoma_7b_yellow.png";
			pathData = "/myfont/tahoma_7b";
			break;
		case 5:
			strFont = str;
			pathImage = "/myfont/tahoma_7b_brown.png";
			pathData = "/myfont/tahoma_7b";
			break;
		case 6:
			strFont = str;
			pathImage = "/myfont/tahoma_7b_green2.png";
			pathData = "/myfont/tahoma_7b";
			break;
		case 7:
			strFont = str;
			pathImage = "/myfont/tahoma_7b_green.png";
			pathData = "/myfont/tahoma_7b";
			break;
		case 8:
			strFont = str;
			pathImage = "/myfont/tahoma_7b_focus.png";
			pathData = "/myfont/tahoma_7b";
			break;
		case 9:
			strFont = str;
			pathImage = "/myfont/tahoma_7b_unfocus.png";
			pathData = "/myfont/tahoma_7b";
			break;
		case 11:
		case 29:
			strFont = str;
			pathImage = "/myfont/tahoma_7_blue1.png";
			pathData = "/myfont/tahoma_7";
			break;
		case 12:
			strFont = str;
			pathImage = "/myfont/tahoma_7_green2.png";
			pathData = "/myfont/tahoma_7";
			break;
		case 13:
			strFont = str;
			pathImage = "/myfont/tahoma_7_yellow.png";
			pathData = "/myfont/tahoma_7";
			break;
		case 14:
		case 31:
			strFont = str;
			pathImage = "/myfont/tahoma_7_grey.png";
			pathData = "/myfont/tahoma_7";
			break;
		case 15:
			strFont = str;
			pathImage = "/myfont/tahoma_7_red.png";
			pathData = "/myfont/tahoma_7";
			break;
		case 16:
			strFont = str;
			pathImage = "/myfont/tahoma_7_blue.png";
			pathData = "/myfont/tahoma_7";
			break;
		case 17:
			strFont = str;
			pathImage = "/myfont/tahoma_7_green.png";
			pathData = "/myfont/tahoma_7";
			break;
		case 18:
			strFont = str;
			pathImage = "/myfont/tahoma_7_white.png";
			pathData = "/myfont/tahoma_7";
			break;
		case 19:
			strFont = str;
			pathImage = "/myfont/tahoma_8b.png";
			pathData = "/myfont/tahoma_8b";
			space = -1;
			break;
		case 20:
		case 26:
			strFont = numberChars;
			pathImage = "/myfont/number_yellow.png";
			pathData = "/myfont/number";
			break;
		case 21:
		case 25:
			strFont = numberChars;
			pathImage = "/myfont/number_red.png";
			pathData = "/myfont/number";
			break;
		case 22:
		case 27:
			strFont = numberChars;
			pathImage = "/myfont/number_green.png";
			pathData = "/myfont/number";
			break;
		case 23:
			strFont = numberChars;
			pathImage = "/myfont/number_gray.png";
			pathData = "/myfont/number";
			break;
		case 24:
		case 30:
			strFont = numberChars;
			pathImage = "/myfont/number_orange.png";
			pathData = "/myfont/number";
			break;
		default:
			strFont = str;
			pathImage = "/myfont/tahoma_7.png";
			pathData = "/myfont/tahoma_7";
			break;
		}
		LoadJavaGlyphMetrics(pathData);
		if (ModGraphics.IsJavaGraphics)
		{
			EnsureJavaImageLoaded();
		}
	}

	private void LoadJavaGlyphMetrics(string dataPath)
	{
		if (string.IsNullOrEmpty(dataPath))
		{
			return;
		}
		if (s_CachedGlyphTables.TryGetValue(dataPath, out int[][] cachedGlyphs) && cachedGlyphs != null)
		{
			fImages = cachedGlyphs;
			javaHeight = s_CachedGlyphHeights.TryGetValue(dataPath, out int ch) ? ch : 11;
			return;
		}
		int[][] parsed = null;
		int maxH = 0;
		DataInputStream dis = null;
		try
		{
			dis = MyStream.readFile(dataPath);
			if (dis != null)
			{
				short count = dis.readShort();
				if (count > 0)
				{
					parsed = new int[count][];
					for (int i = 0; i < count; i++)
					{
						parsed[i] = new int[4];
						parsed[i][0] = dis.readShort();
						parsed[i][1] = dis.readShort();
						parsed[i][2] = dis.readShort();
						parsed[i][3] = dis.readShort();
						if (parsed[i][3] > maxH)
						{
							maxH = parsed[i][3];
						}
					}
				}
			}
		}
		catch (Exception)
		{
			parsed = null;
		}
		finally
		{
			try
			{
				dis?.close();
			}
			catch (Exception)
			{
			}
		}

		if (parsed == null || parsed.Length == 0)
		{
			string b64 = dataPath switch
			{
				"/myfont/tahoma_7b" => JavaDataTahoma7bBase64,
				"/myfont/tahoma_8b" => JavaDataTahoma8bBase64,
				"/myfont/number" => JavaDataNumberBase64,
				_ => JavaDataTahoma7Base64
			};
			byte[] raw = Convert.FromBase64String(b64);
			int count = (raw[0] << 8) | raw[1];
			parsed = new int[count][];
			maxH = 0;
			int pos = 2;
			for (int i = 0; i < count && pos + 7 < raw.Length; i++)
			{
				parsed[i] = new int[4];
				parsed[i][0] = (short)((raw[pos] << 8) | raw[pos + 1]);
				parsed[i][1] = (short)((raw[pos + 2] << 8) | raw[pos + 3]);
				parsed[i][2] = (short)((raw[pos + 4] << 8) | raw[pos + 5]);
				parsed[i][3] = (short)((raw[pos + 6] << 8) | raw[pos + 7]);
				if (parsed[i][3] > maxH)
				{
					maxH = parsed[i][3];
				}
				pos += 8;
			}
		}

		fImages = parsed;
		javaHeight = maxH > 0 ? maxH : 11;
		s_CachedGlyphTables[dataPath] = parsed;
		s_CachedGlyphHeights[dataPath] = javaHeight;
	}

	private Image EnsureJavaImageLoaded()
	{
		if (imgFont != null && imgFont.texture != null)
		{
			return imgFont;
		}
		if (string.IsNullOrEmpty(pathImage))
		{
			return null;
		}
		try
		{
			imgFont = Image.createImage(GameCanvas.cutPng(Main.res + "/x1" + pathImage));
			if (imgFont == null)
			{
				imgFont = GameCanvas.loadImage(pathImage);
			}
		}
		catch (Exception)
		{
		}
		return imgFont;
	}

	private int ResolveJavaGlyphIndex(char c)
	{
		if (string.IsNullOrEmpty(strFont) || fImages == null || fImages.Length == 0)
		{
			return 0;
		}
		int idx = strFont.IndexOf(c);
		if (idx >= 0 && idx < fImages.Length)
		{
			return idx;
		}
		char mapped = c switch
		{
			'|' => 'I',
			';' => ':',
			'"' => '\'',
			'“' => '\'',
			'”' => '\'',
			'‘' => '\'',
			'’' => '\'',
			'~' => '-',
			'–' => '-',
			'—' => '-',
			'\\' => '/',
			_ => '\0'
		};
		if (mapped != '\0')
		{
			idx = strFont.IndexOf(mapped);
			if (idx >= 0 && idx < fImages.Length)
			{
				return idx;
			}
		}
		return 0;
	}

	public static void ReloadJavaFontImages()
	{
		for (int i = 0; i < s_AllInstances.Count; i++)
		{
			mFont f = s_AllInstances[i];
			if (f != null)
			{
				f.imgFont = null;
				if (ModGraphics.IsJavaGraphics)
				{
					f.EnsureJavaImageLoaded();
				}
			}
		}
		InvalidateFontMetrics();
	}

	public static void init()
		{
			s_AllInstances.Clear();
			if (mGraphics.zoomLevel == 1)
			{
				tahoma_7b_red = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7b_red.png", "/myfont/tahoma_7b", 0);
				tahoma_7b_blue = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7b_blue.png", "/myfont/tahoma_7b", 0);
				tahoma_7b_white = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7b_white.png", "/myfont/tahoma_7b", 0);
				tahoma_7b_yellow = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7b_yellow.png", "/myfont/tahoma_7b", 0);
				tahoma_7b_yellowSmall = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7b_yellow.png", "/myfont/tahoma_7b", 0);
				tahoma_7b_dark = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7b_brown.png", "/myfont/tahoma_7b", 0);
				tahoma_7b_green2 = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7b_green2.png", "/myfont/tahoma_7b", 0);
				tahoma_7b_green = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7b_green.png", "/myfont/tahoma_7b", 0);
				tahoma_7b_focus = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7b_focus.png", "/myfont/tahoma_7b", 0);
				tahoma_7b_unfocus = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7b_unfocus.png", "/myfont/tahoma_7b", 0);
				tahoma_7 = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7.png", "/myfont/tahoma_7", 0);
				tahoma_7_blue1 = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7_blue1.png", "/myfont/tahoma_7", 0);
				tahoma_7_green2 = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7_green2.png", "/myfont/tahoma_7", 0);
				tahoma_7_yellow = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7_yellow.png", "/myfont/tahoma_7", 0);
				tahoma_7_grey = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7_grey.png", "/myfont/tahoma_7", 0);
				tahoma_7_red = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7_red.png", "/myfont/tahoma_7", 0);
				tahoma_7_blue = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7_blue.png", "/myfont/tahoma_7", 0);
				tahoma_7_green = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7_green.png", "/myfont/tahoma_7", 0);
				tahoma_7_white = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_7_white.png", "/myfont/tahoma_7", 0);
				tahoma_8b = new mFont(" 0123456789+-*='_?.,<>/[]{}!@#$%^&*():aáàảãạâấầẩẫậăắằẳẵặbcdđeéèẻẽẹêếềểễệfghiíìỉĩịjklmnoóòỏõọôốồổỗộơớờởỡợpqrstuúùủũụưứừửữựvxyýỳỷỹỵzwAÁÀẢÃẠĂẰẮẲẴẶÂẤẦẨẪẬBCDĐEÉÈẺẼẸÊẾỀỂỄỆFGHIÍÌỈĨỊJKLMNOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢPQRSTUÚÙỦŨỤƯỨỪỬỮỰVXYÝỲỶỸỴZW", "/myfont/tahoma_8b.png", "/myfont/tahoma_8b", -1);
				number_yellow = new mFont(" 0123456789+-", "/myfont/number_yellow.png", "/myfont/number", 0);
				number_red = new mFont(" 0123456789+-", "/myfont/number_red.png", "/myfont/number", 0);
				number_green = new mFont(" 0123456789+-", "/myfont/number_green.png", "/myfont/number", 0);
				number_gray = new mFont(" 0123456789+-", "/myfont/number_gray.png", "/myfont/number", 0);
				number_orange = new mFont(" 0123456789+-", "/myfont/number_orange.png", "/myfont/number", 0);
				bigNumber_red = number_red;
				bigNumber_While = tahoma_7b_white;
				bigNumber_yellow = number_yellow;
				bigNumber_green = number_green;
				bigNumber_orange = number_orange;
				bigNumber_blue = tahoma_7_blue1;
				nameFontRed = tahoma_7_red;
				nameFontYellow = tahoma_7_yellow;
				nameFontGreen = tahoma_7_green;
				tahoma_7_greySmall = tahoma_7_grey;
				tahoma_7b_yellowSmall2 = tahoma_7_yellow;
				tahoma_7b_green2Small = tahoma_7b_green2;
				tahoma_7_whiteSmall = tahoma_7_white;
				tahoma_7b_greenSmall = tahoma_7b_green;
				tahoma_7_blue1Small = tahoma_7_blue1;
				return;
			}
			gI = new mFont(0);
			tahoma_7b_red = new mFont(1);
			tahoma_7b_blue = new mFont(2);
			tahoma_7b_white = new mFont(3);
			tahoma_7b_yellow = new mFont(4);
			tahoma_7b_yellowSmall = new mFont(4);
			tahoma_7b_dark = new mFont(5);
			tahoma_7b_green2 = new mFont(6);
			tahoma_7b_green = new mFont(7);
			tahoma_7b_focus = new mFont(8);
			tahoma_7b_unfocus = new mFont(9);
			tahoma_7 = new mFont(10);
			tahoma_7_blue1 = new mFont(11);
			tahoma_7_blue1Small = tahoma_7_blue1;
			tahoma_7_green2 = new mFont(12);
			tahoma_7_yellow = new mFont(13);
			tahoma_7_grey = new mFont(14);
			tahoma_7_red = new mFont(15);
			tahoma_7_blue = new mFont(16);
			tahoma_7_green = new mFont(17);
			tahoma_7_white = new mFont(18);
			tahoma_8b = new mFont(19);
			number_yellow = new mFont(20);
			number_red = new mFont(21);
			number_green = new mFont(22);
			number_gray = new mFont(23);
			number_orange = new mFont(24);
			bigNumber_red = new mFont(25);
			bigNumber_yellow = new mFont(26);
			bigNumber_green = new mFont(27);
			bigNumber_While = new mFont(28);
			bigNumber_blue = new mFont(29);
			bigNumber_orange = new mFont(30);
			bigNumber_black = new mFont(31);
			nameFontRed = tahoma_7b_red;
			nameFontYellow = tahoma_7_yellow;
			nameFontGreen = tahoma_7_green;
			tahoma_7_greySmall = tahoma_7_grey;
			tahoma_7b_yellowSmall2 = tahoma_7_yellow;
			tahoma_7b_green2Small = tahoma_7b_green2;
			tahoma_7_whiteSmall = tahoma_7_white;
			tahoma_7b_greenSmall = tahoma_7b_green;
			yAddFont = 1;
			if (mGraphics.zoomLevel == 1)
			{
				yAddFont = -3;
			}
		}

	public Color setColor(int rgb)
		{
			int num = rgb & 0xFF;
			int num2 = (rgb >> 8) & 0xFF;
			int num3 = (rgb >> 16) & 0xFF;
			float b = (float)num / 256f;
			float g = (float)num2 / 256f;
			float r = (float)num3 / 256f;
			return new Color(r, g, b);
		}

	public Color bigColor(int id)
		{
			Color[] array = new Color[7]
			{
				Color.red,
				Color.yellow,
				Color.green,
				Color.white,
				setColor(40404),
				Color.red,
				Color.black
			};
			return array[id - 25];
		}

	public void setColorByID(int ID)
		{
			color1 = setColor(colorJava[ID]);
			color2 = setColor(colorJava[ID]);
		}

	public Color setColorFont(sbyte id)
		{
			return setColor(colorJava[id]);
		}

	public void reloadImage()
		{
			if (mGraphics.zoomLevel == 1 || ModGraphics.IsJavaGraphics)
			{
				imgFont = null;
				EnsureJavaImageLoaded();
			}
		}

	public void freeImage()
		{
		}

	public static mFont GetFont(sbyte colorCode)
		{
			return colorCode switch
			{
				0 => tahoma_7b_dark, 
				1 => tahoma_7b_green, 
				2 => tahoma_7b_blue, 
				3 => tahoma_7_red, 
				4 => tahoma_7_green, 
				5 => tahoma_7_blue, 
				7 => tahoma_7b_red, 
				_ => tahoma_7, 
			};
		}

}
