Select distinct TOP 100
'G�ska sp. z o.o.' as [partner],
case when Twr_PrdNumer = 19468 then 'JAG-PREMIUM' else 'JAG' end as [brand],
'Aftermarket' as [sparepart_type],
Twr_Kod as [product_sku],
Twr_Nazwa as [title],

--Grupuje klase oraz wartosci atrubut�w jakie posiada produkt. Warunek taki, �e grupa klas atrybut�w, to wymiary towaru
isnull(STUFF(
                 (SELECT distinct ', ' + AtK_Nazwa + ': ' + Atr_Wartosc from cdn.TwrKarty US with(nolock)
				 join cdn.Atrybuty with(nolock) on Twr_GIDNumer=Atr_ObiNumer and Atr_OBITyp=16 and Atr_OBILp = 0
				 join cdn.AtrybutyKlasy with(nolock) on  AtK_ID=Atr_AtkId
				 join cdn.AtrKompletyLinki with (nolock) on AtK_ID=AKl_AtKId
				 --left join cdn.TwrOpisy with(nolock) on Twr_GIDNumer=TwO_TwrNumer
					where US.Twr_Kod = SS.twr_kod
					and AKl_AKpID = 2

                           FOR XML PATH ('')), 1, 1, ''
               ),'') as [description],

cast((twC_wartosc/4.7) as decimal(10,2)) as [cost],
cast((twC_wartosc*1.35)/4.7 as decimal(10,2)) as [price],
guarantee_y = 2,
Twr_Ean as [ean_code],
Twr_Waga as [total_weight_kg],
country_origin = 'Poland',
delivery_de_delay = '2-3 days',
delivery_at_delay = '2-3 days',
delivery_fr_delay = '4-5 days',
delivery_be_delay = '3-4 days',
TPO_OpisKrotki as [oem_number],


--Je�li towar jest w grupie '6.1 Cz�ci wed�ug rodzaju' to outputtuje �w rodzaj, a je�li nie jest podpi�ty do takiej grupy to nic
isnull((Select top 1 REVERSE(SUBSTRING(REVERSE(CDN.TwrGrupaPelnaNazwa(Twg_GRONumer)), 0, CHARINDEX('/', REVERSE(CDN.TwrGrupaPelnaNazwa(Twg_GRONumer)))))
from cdn.twrgrupy KS with(nolock)
where ks.TwG_GIDNumer = kk.TwG_GIDNumer
and CDN.TwrGrupaPelnaNazwa(Twg_GRONumer) like '6.1%'),'') as [category],



--Grupuje oraz Outputtuje to co jest pomiedzy 2-gim a 3-cim '/' w pe�nej nazwie grupy, czyli do jakiej maszyny/pojazdu s�u�y np. 'Ci�gnik', 'Prasa' itp. 
STUFF((Select distinct ','+SUBSTRING(CDN.TwrGrupaPelnaNazwa(Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer)) + 1) + 1, 
    CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer)) + 1) + 1) - 
    CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer)) + 1) - 1)
	from cdn.twrgrupy KS with(nolock)
	where ks.TwG_GIDNumer = kk.TwG_GIDNumer
	and CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer)) + 1) > 0
	and TwG_GrONumer BETWEEN 36501 AND 53404
	and CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer)) + 1) + 1) > 0
	FOR XML PATH ('')), 1, 1, '') AS [model_vehicle_code],


--Outputuje to co jest pomiedzy 1-szym a 2-gim '/' w pe�nej nazwie grupy czyli marke np. 'CLASS','URSUS' itp.
SUBSTRING(CDN.TwrGrupaPelnaNazwa(Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer)) + 1, CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer)) + 1) - CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer)) - 1) AS [model_brand_name],


--Grupuje Wszystkie ID Modeli (to co jest po ostatnim '/' w pe�nej nazwie grupy)
STUFF((Select ',' + REVERSE(SUBSTRING(REVERSE(CDN.TwrGrupaPelnaNazwa(Twg_GRONumer)), 0, CHARINDEX('/', REVERSE(CDN.TwrGrupaPelnaNazwa(Twg_GRONumer)))))
from cdn.twrgrupy KS with(nolock)
where ks.TwG_GIDNumer = kk.TwG_GIDNumer
and TwG_GrONumer BETWEEN 36501 AND 53404
and CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer)) + 1) > 0
and SUBSTRING(CDN.TwrGrupaPelnaNazwa(ks.Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(ks.Twg_GRONumer)) + 1, CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(ks.Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(ks.Twg_GRONumer)) + 1) - CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(ks.Twg_GRONumer)) - 1)
= SUBSTRING(CDN.TwrGrupaPelnaNazwa(kk.Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(kk.Twg_GRONumer)) + 1, CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(kk.Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(kk.Twg_GRONumer)) + 1) - CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(kk.Twg_GRONumer)) - 1)
FOR XML PATH ('')), 1, 1, '') as [model_ID],

--Grupuje linki do zdj��
isnull(STUFF((Select ', ' + 'https://www.b2b.gaska.com.pl/img/produkty/'+convert(varchar,twr_gidnumer) +'/'+convert(varchar,dab_ID)+'_'+DAB_Nazwa+'.jpg'
from cdn.TwrKarty us with(nolock)
join cdn.DaneObiekty with(nolock) on Twr_GIDNumer=DAO_ObiNumer and DAO_ObiTyp=16
join cdn.DaneBinarne with(nolock) on DAB_ID=DAO_DABId
and ss.Twr_GIDNumer = us.Twr_GIDNumer
and DAB_Rozszerzenie = 'jpg'
FOR XML PATH ('')), 1, 1, ''),'') as [image_links]


from cdn.TwrKarty SS with(nolock)
join cdn.TwrCeny with(nolock) on Twr_GIDNumer=TwC_TwrNumer
join cdn.Atrybuty with(nolock) on Twr_GIDNumer=Atr_ObiNumer and Atr_OBITyp=16 and Atr_OBILp = 0
join cdn.AtrybutyKlasy with(nolock) on  AtK_ID=Atr_AtkId
--left join cdn.TwrOpisy with(nolock) on Twr_GIDNumer=TwO_TwrNumer
join cdn.TwrAplikacjeOpisy with(nolock) on Twr_GIDTyp=TPO_ObiTyp AND Twr_GIDNumer=TPO_ObiNumer	and Twr_GIDTyp=16
join cdn.twrgrupy KK with(nolock) on Twr_GIDTyp=TwG_GIDTyp AND Twr_GIDNumer=TwG_GIDNumer and TwG_GIDTyp=16

where TwC_TwrLp = 3
and Twr_PrdNumer in (19468,19467)
and TPO_JezykId = 0
and Atr_Wartosc = 'Standardowy'
and TwG_GrONumer BETWEEN 36501 AND 53404
and CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer), CHARINDEX('/', CDN.TwrGrupaPelnaNazwa(Twg_GRONumer)) + 1) > 0
--and AtK_ID in (28,25,29)








