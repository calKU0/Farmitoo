select
twr_kod as [product_sku],
twr_ean as [product_ean],
isnull('https://www.b2b.gaska.com.pl/img/produkty/'+convert(varchar,twr_gidnumer) +'/'+convert(varchar,dab_ID)+'_'+DAB_Nazwa+'.jpg','') as [image link]
from cdn.TwrKarty us with(nolock)
join cdn.Atrybuty with(nolock) on Twr_GIDNumer=Atr_ObiNumer and Atr_OBITyp=16 and Atr_OBILp = 0
join cdn.AtrybutyKlasy with(nolock) on  AtK_ID=Atr_AtkId
left join cdn.DaneObiekty with(nolock) on Twr_GIDNumer=DAO_ObiNumer and DAO_ObiTyp=16
left join cdn.DaneBinarne with(nolock) on DAB_ID=DAO_DABId
where Twr_PrdNumer in (19468,19467)
and Atr_Wartosc = 'Standardowy'
and DAB_Rozszerzenie = 'jpg'
and Twr_GIDNumer<=4176
