class InstagramVideo {
    constructor() { }


    static enableControl(document) {
        const observer = new MutationObserver(mutations => {
            for (const x of mutations) {
                if (x.type !== 'childList')
                    continue;
                this.enablePostsVideoControl(x.target);
                this.enableHighlightsVideoControl(x.target);
            }
        });
        observer.observe(document, { childList: true, subtree: true });
    }


    static enablePostsVideoControl(parent) {
        const videos = this.findElements(parent, 'video.tWeCl');
        if (videos.length > 0) {
            videos.prop('controls', true);
            this.findElements(parent, 'img._8jZFn').hide();
            this.findElements(parent, 'div.PyenC').hide();
            this.findElements(parent, 'div.fXIG0').hide();
            console.log('Enable post video control.');
        }
    }


    static enableHighlightsVideoControl(parent) {
        const videos = this.findElements(parent, 'video.y-yJ5');
        if (videos.length > 0) {
            this.findElements(parent, 'div.yxA_V').css('pointer-events', 'auto');
            videos.prop('autoplay', false);
            videos.prop('controls', true);
            videos.removeAttr('playsinline');
            videos.removeClass('OFkrO');
            videos.siblings('div.i0EQd').hide();
            console.log('Enable highlight video control.');
        }
    }


    static findElements(parent, selector) {
        return parent === undefined || parent === null
            ? $(selector)
            : $(parent).find(selector);
    }
}


class KKSearch {
    constructor() { }


    static isAvailable(url) {
        const targetUrl = 'https://www.instagram.com/kk_talking';
        return url?.toLowerCase()?.startsWith(targetUrl) ?? false;
    }


    static enable(document) {
        const observer = new MutationObserver(mutations => {
            if (!KKSearch.isAvailable(location.href))
                return;

            for (const x of mutations) {
                if (x.type === 'childList')
                    this.tryAddSearchTab();
            }
        });
        observer.observe(document, { childList: true, subtree: true });
    }


    static tryAddSearchTab() {
        //--- タブコンテナー要素が生成されているか
        const tabContainer = $('div.fx7hk');
        if (tabContainer.length === 0)
            return;

        //--- 検索タブがあるか
        if (tabContainer.children('#kk_searchTab').length > 0)
            return;

        //--- なければ検索タブを生成
        const searchTabDomText = '<a id="kk_searchTab" class="_9VEo1" href="/kk_talking#search"><span class="qzihg"><div class="coreSpriteSearchIcon"></div><span class="_08DtY">KK Search</span></span></a>';
        const searchTabContentDomText
            = '<div id="kk_searchTabContent">'
            + '<div class="kk_searchBox"><form><input class="XTCLo x3qfX" type="text" placeholder="Input topic or tips here."></form></div>'
            + '<div style="border: 1px solid">ここの下に検索結果を表示する</div>'
            + '</div>';
        tabContainer.append(searchTabDomText);
        tabContainer.after(searchTabContentDomText);

        //--- タブの On / Off を制御
        const tabs = tabContainer.children('a._9VEo1');
        tabs.on('click', e => {
            const activeTabClassName = 'T-jvg';
            tabs.removeClass(activeTabClassName);  // 一旦全タブを無効化

            const searchTabContent = $('#kk_searchTabContent');
            const otherTabContent = tabContainer.siblings('div._2z6nI');
            if (e.currentTarget.id === 'kk_searchTab') {
                $(e.currentTarget).addClass(activeTabClassName);  // 検索タブを有効化
                searchTabContent.show();
                otherTabContent.hide();
                return false;  // a タグを無効化
            }
            else {
                searchTabContent.hide();
                otherTabContent.show();
            }
        });

        //--- 検索フォームのイベント
        this.attachSearchBoxEvent();
    }


    static attachSearchBoxEvent() {
        $('.kk_searchBox form').on('submit', async e =>
        {
            e.preventDefault();
            try
            {
                const textBox = $(e.target).find('input[type="text"]');
                console.log(textBox.val());

                // todo: ajax で Azure Functions に問い合わせ
                const task = new Promise(resolve =>
                {
                    resolve({
                        id: 76,
                        topic: 'when it comes to',
                        tips: [
                            'aiueo',
                            'abcde',
                        ]
                    });
                });
                var result = await task;
                console.log(result);

                // todo:
                // 検索に成功したら
                // 1. 要素消して
                // 2. 要素を新規追加
            }
            catch (ex)
            {
                console.error(ex);
            }
        });
    }
}


(function () {
    InstagramVideo.enableControl(document);
    KKSearch.enable(document);
}());
