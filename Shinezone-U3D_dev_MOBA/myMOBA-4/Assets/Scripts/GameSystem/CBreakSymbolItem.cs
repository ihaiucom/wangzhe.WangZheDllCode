using System;

namespace Assets.Scripts.GameSystem
{
	public class CBreakSymbolItem : IComparable
	{
		public CSymbolItem symbol;

		public bool bBreak;

		public bool bBreakToggle;

		public CBreakSymbolItem(CSymbolItem tempSymbol, bool btempBreak)
		{
			this.symbol = tempSymbol;
			this.bBreak = btempBreak;
			this.bBreakToggle = btempBreak;
		}

		public int CompareTo(object obj)
		{
			CBreakSymbolItem cBreakSymbolItem = obj as CBreakSymbolItem;
			if (this.symbol.m_SymbolData.wLevel < cBreakSymbolItem.symbol.m_SymbolData.wLevel)
			{
				return 1;
			}
			if (this.symbol.m_SymbolData.wLevel > cBreakSymbolItem.symbol.m_SymbolData.wLevel)
			{
				return -1;
			}
			if (this.symbol.m_SymbolData.bColor != cBreakSymbolItem.symbol.m_SymbolData.bColor)
			{
				return (this.symbol.m_SymbolData.bColor <= cBreakSymbolItem.symbol.m_SymbolData.bColor) ? -1 : 1;
			}
			if (this.symbol.m_SymbolData.dwID == cBreakSymbolItem.symbol.m_SymbolData.dwID)
			{
				return 0;
			}
			return (this.symbol.m_SymbolData.dwID <= cBreakSymbolItem.symbol.m_SymbolData.dwID) ? -1 : 1;
		}
	}
}
