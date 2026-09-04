public class ItemOption
{
	public int param;

	public sbyte active;

	public sbyte activeCard;

	public ItemOptionTemplate optionTemplate;

	public ItemOption()
	{
	}

	public ItemOption(int optionTemplateId, int param)
	{
		if (optionTemplateId == 22)
		{
			optionTemplateId = 6;
			param *= 1000;
		}
		if (optionTemplateId == 23)
		{
			optionTemplateId = 7;
			param *= 1000;
		}
		this.param = param;
		optionTemplate = GameScr.gI().iOptionTemplates[optionTemplateId];
	}

	public string getOptionString()
	{
		string text = (optionTemplate != null) ? Res.GetVietnameseOptionTemplate(optionTemplate.id, optionTemplate.name) : string.Empty;
		return NinjaUtil.replace(text, "#", param + string.Empty);
	}

	public string getOptionName()
	{
		string text = (optionTemplate != null) ? Res.GetVietnameseOptionTemplate(optionTemplate.id, optionTemplate.name) : string.Empty;
		return NinjaUtil.replace(text, "+#", string.Empty);
	}

	public string getOptiongColor()
	{
		string text = (optionTemplate != null) ? Res.GetVietnameseOptionTemplate(optionTemplate.id, optionTemplate.name) : string.Empty;
		return NinjaUtil.replace(text, "$", string.Empty);
	}
}
