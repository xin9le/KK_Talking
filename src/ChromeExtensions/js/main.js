class InstagramVideo {
    constructor() { }


    static enableControl(document) {
        const observer = new MutationObserver(mutations => {
            for (const x of mutations) {
                if (x.type !== 'childList')
                    continue;
                this.enablePostsVideoControl(null);
                this.enableHighlightsVideoControl(null);
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
        const searchTabDomText =
            `<a id="kk_searchTab" class="_9VEo1" href="/kk_talking#search">
                <span class="qzihg">
                    <div class="coreSpriteSearchIcon"></div>
                    <span class="_08DtY">KK Search</span>
                </span>
            </a>`;
        const searchTabContentDomText =
            `<div id="kk_searchTabContent">
                <div class="kk_searchBox">
                    <form>
                        <input class="XTCLo x3qfX" type="text" placeholder="Input topic or tips here.">
                    </form>
                </div>
                <div id="kk_searchResultContainer"></div>
            </div>`;
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
        $('.kk_searchBox form').on('submit', async e => {
            e.preventDefault();
            const container = $('#kk_searchResultContainer');
            try {
                //--- 検索
                const textBox = $(e.target).find('input[type="text"]');
                const keyword = encodeURIComponent(textBox.val());
                const url = 'https://kktalking.azure-api.net/instagram/v1/search?q=' + keyword;
                const result = await $.get(url);

                //--- 要素を削除
                container.empty();

                //--- 要素を追加
                for (const x of result) {
                    console.log(x);

                    //--- Topics
                    let topics = '';
                    if (0 < x.topics.length)
                    {
                        topics = '<div class="kk_topic"><div>⚜️Topic</div><dl>';
                        for (const t of x.topics)
                        {
                            topics +=
                                `<dt>${t.english}</dt>
                                <dd>${t.japanese}</dd>`;
                        }
                        topics += '</dl></div>';
                    }

                    //--- Tips
                    let tips = '';
                    if (0 < x.tips.length)
                    {
                        tips = '<div class="kk_topic"><div>🍀Tips</div><dl>';
                        for (const t of x.tips)
                        {
                            tips +=
                                `<dt>${t.english}</dt>
                                <dd>${t.japanese}</dd>`;
                        }
                        tips += '</dl></div>';
                    }

                    //--- 要素
                    const item =
                        `<div class="kk_itemBox">
                            <div class="kk_imageBox">
                                <a href="https://www.instagram.com/p/${x.shortCode}" target="_blank">
                                    <img src="${x.imageUrl}" />
                                </a>
                            </div>
                            <div class="kk_infoBox">
                                <a href="https://www.instagram.com/p/${x.shortCode}" target="_blank"><h1>KK ${x.number}</h1></a>
                                ${topics}
                                ${tips}
                            </div>
                        </div>`;
                    container.append(item);
                }
            }
            catch (ex) {
                console.error(ex);
            }
        });
    }
}


(function () {
    InstagramVideo.enableControl(document);
    KKSearch.enable(document);
}());
