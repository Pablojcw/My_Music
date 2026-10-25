const pageContent = document.querySelector("#page-content");
const playerPanel = document.querySelector("#player-panel");
const playerBackdrop = document.querySelector("#player-backdrop");
const playerFrame = document.querySelector("#player-frame");
const playerTitle = document.querySelector("#player-title");
const playerTrackTitle = document.querySelector("#player-track-title");
const playerTrackChannel = document.querySelector("#player-track-channel");
const playerTrackArt = document.querySelector("#player-track-art");
const savedVideoIds = new Set();
let youtubePlayer = null;
let pendingYouTubeVideo = null;
let playerElementId = 0;
let hasActivePlayback = false;
let libraryItems = [];

window.onYouTubeIframeAPIReady = () => {
  if (pendingYouTubeVideo) {
    mountYouTubePlayer(pendingYouTubeVideo);
  }
};

function readRoute() {
  const hash = window.location.hash || "#inicio";
  const libraryMatch = hash.match(/^#biblioteca\/canal\/(.+)$/);
  if (libraryMatch) {
    try {
      return { page: "biblioteca", channel: decodeURIComponent(libraryMatch[1]) };
    } catch {
      return { page: "biblioteca", channel: null };
    }
  }

  const page = hash.slice(1);
  return { page: ["inicio", "buscar", "biblioteca"].includes(page) ? page : "inicio" };
}

function setMessage(element, message, kind = "") {
  if (!element) return;
  element.textContent = message;
  element.className = `status-message${kind ? ` ${kind}` : ""}`;
}

async function readResponse(response) {
  const contentType = response.headers.get("content-type") ?? "";
  const payload = contentType.includes("json") ? await response.json() : null;
  if (!response.ok) {
    throw new Error(payload?.detail || "Não foi possível concluir esta ação.");
  }
  return payload;
}

function makeFallback() {
  const fallback = document.createElement("span");
  fallback.className = "thumb-fallback";
  fallback.setAttribute("aria-hidden", "true");
  fallback.textContent = "♫";
  return fallback;
}

function makeThumbnail(url, title, compact = false) {
  const wrapper = document.createElement("div");
  wrapper.className = compact ? "library-thumb" : "media-thumb";
  if (url && /^https:\/\//i.test(url)) {
    const image = document.createElement("img");
    image.src = url;
    image.alt = "";
    image.loading = "lazy";
    image.referrerPolicy = "no-referrer";
    image.addEventListener("error", () => wrapper.replaceChildren(makeFallback()), { once: true });
    wrapper.append(image);
  } else {
    wrapper.append(makeFallback());
  }

  if (!compact) {
    wrapper.setAttribute("role", "button");
    wrapper.setAttribute("tabindex", "0");
    wrapper.setAttribute("aria-label", `Reproduzir ${title}`);
    const play = document.createElement("span");
    play.className = "play-overlay";
    play.setAttribute("aria-hidden", "true");
    play.textContent = "▶";
    wrapper.append(play);
  }
  return wrapper;
}

function makeResultCard(video) {
  const card = document.createElement("article");
  card.className = "media-card";
  card.dataset.videoId = video.videoId;
  const thumbnail = makeThumbnail(video.thumbnailUrl, video.title);
  const play = () => createYouTubePlayer(video.videoId, video.title, video.channelTitle, video.thumbnailUrl);
  thumbnail.addEventListener("click", play);
  thumbnail.addEventListener("keydown", (event) => {
    if (event.key === "Enter" || event.key === " ") {
      event.preventDefault();
      play();
    }
  });

  const body = document.createElement("div");
  body.className = "media-card-body";
  const title = document.createElement("h3");
  title.className = "media-title";
  title.textContent = video.title;
  const channel = document.createElement("span");
  channel.className = "media-meta";
  channel.textContent = video.channelTitle || "Canal não informado";

  const actions = document.createElement("div");
  actions.className = "media-actions";
  const playButton = document.createElement("button");
  playButton.className = "action-button primary";
  playButton.type = "button";
  playButton.textContent = "▶ Reproduzir";
  playButton.addEventListener("click", play);
  const saveButton = document.createElement("button");
  saveButton.className = "action-button";
  saveButton.type = "button";
  saveButton.textContent = savedVideoIds.has(video.videoId) ? "Salvo" : "＋ Salvar na biblioteca";
  saveButton.disabled = savedVideoIds.has(video.videoId);
  saveButton.addEventListener("click", () => saveVideo(video, saveButton));
  actions.append(playButton, saveButton);
  body.append(title, channel, actions);
  card.append(thumbnail, body);
  return card;
}

function createYouTubePlayer(videoId, title, channel, thumbnail) {
  if (!/^[A-Za-z0-9_-]{11}$/.test(videoId)) return;
  pendingYouTubeVideo = { videoId, title, channel, thumbnail };
  hasActivePlayback = true;
  playerPanel.classList.remove("idle");
  playerPanel.classList.add("open");
  playerPanel.classList.remove("docked");
  playerBackdrop.classList.add("visible");
  playerPanel.classList.remove("large");
  playerPanel.setAttribute("aria-modal", "true");
  playerPanel.setAttribute("role", "dialog");
  playerPanel.setAttribute("aria-label", `Reproduzindo ${title}`);
  playerTitle.textContent = title;
  playerTrackTitle.textContent = title;
  playerTrackChannel.textContent = `${channel || "Canal não informado"} · vídeo do YouTube`;
  playerTrackArt.replaceChildren();

  if (thumbnail && /^https:\/\//i.test(thumbnail)) {
    const image = document.createElement("img");
    image.src = thumbnail;
    image.alt = "";
    image.referrerPolicy = "no-referrer";
    image.addEventListener("error", () => playerTrackArt.replaceChildren(makeFallback()), { once: true });
    playerTrackArt.append(image);
  } else {
    playerTrackArt.append(makeFallback());
  }

  if (youtubePlayer) {
    youtubePlayer.destroy();
    youtubePlayer = null;
  }

  playerFrame.replaceChildren();
  const loading = document.createElement("div");
  loading.className = "player-placeholder";
  loading.textContent = "Carregando o player oficial...";
  playerFrame.append(loading);
  if (window.YT?.Player) {
    mountYouTubePlayer(pendingYouTubeVideo);
  } else {
    const selectedVideo = pendingYouTubeVideo;
    setTimeout(() => {
      if (pendingYouTubeVideo === selectedVideo && !window.YT?.Player) {
        loading.textContent = "Não foi possível carregar o player oficial.";
      }
    }, 12000);
  }
}

function mountYouTubePlayer(video) {
  if (!window.YT?.Player || pendingYouTubeVideo !== video) return;
  playerFrame.replaceChildren();
  const target = document.createElement("div");
  target.id = `youtube-player-${++playerElementId}`;
  playerFrame.append(target);
  youtubePlayer = new window.YT.Player(target.id, {
    width: "100%",
    height: "100%",
    videoId: video.videoId,
    playerVars: {
      autoplay: 1,
      playsinline: 1,
      enablejsapi: 1,
      origin: window.location.origin
    },
    events: {
      onError: (event) => {
        if (pendingYouTubeVideo !== video) return;
        const message = event.data === 101 || event.data === 150
          ? "O responsável pelo vídeo não permite reprodução incorporada."
          : "Este vídeo está indisponível no player do YouTube.";
        playerFrame.replaceChildren(makeFallback());
        playerFrame.firstElementChild?.classList.add("player-placeholder");
        playerFrame.firstElementChild.textContent = message;
      }
    }
  });
}

function minimizePlayer() {
  if (!hasActivePlayback) return;
  playerPanel.classList.remove("open", "large");
  playerPanel.classList.add("docked");
  playerBackdrop.classList.remove("visible");
  playerPanel.removeAttribute("aria-modal");
  playerPanel.setAttribute("aria-label", "Mini-player do SoundSpace");
}

function expandPlayer() {
  if (!hasActivePlayback) return;
  playerPanel.classList.add("open");
  playerPanel.classList.remove("docked");
  playerBackdrop.classList.add("visible");
  playerPanel.setAttribute("aria-modal", "true");
  playerPanel.setAttribute("aria-label", `Reproduzindo ${pendingYouTubeVideo.title}`);
}

function stopPlayer() {
  if (youtubePlayer) {
    youtubePlayer.destroy();
    youtubePlayer = null;
  }
  pendingYouTubeVideo = null;
  hasActivePlayback = false;
  playerPanel.classList.remove("open", "large");
  playerPanel.classList.add("docked");
  playerPanel.classList.add("idle");
  playerBackdrop.classList.remove("visible");
  playerPanel.removeAttribute("aria-modal");
  playerTitle.textContent = "Escolha um som";
  playerTrackTitle.textContent = "Nada selecionado";
  playerTrackChannel.textContent = "O player oficial do YouTube aparecerá aqui";
  playerTrackArt.textContent = "♫";
  playerFrame.innerHTML = '<div class="player-placeholder"><span class="placeholder-disc" aria-hidden="true">♫</span><p>Escolha um vídeo para começar.</p></div>';
}

async function loadLibraryItems() {
  libraryItems = await readResponse(await fetch("/api/biblioteca"));
  savedVideoIds.clear();
  for (const item of libraryItems) {
    if (item.musica?.videoExternoId) savedVideoIds.add(item.musica.videoExternoId);
  }
  return libraryItems;
}

async function saveVideo(video, button) {
  const originalText = button.textContent;
  button.disabled = true;
  button.textContent = "Salvando...";
  const status = document.querySelector("#search-status");
  setMessage(status, "");
  try {
    await readResponse(await fetch("/api/music-search/biblioteca", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ videoId: video.videoId })
    }));
    savedVideoIds.add(video.videoId);
    button.textContent = "Salvo";
    setMessage(status, "Vídeo salvo na sua biblioteca.", "success");
  } catch (error) {
    button.disabled = false;
    button.textContent = originalText;
    setMessage(status, error.message, "error");
  }
}

async function renderSearchPage() {
  const form = document.querySelector("#search-form");
  const input = document.querySelector("#search-input");
  const status = document.querySelector("#search-status");
  const resultsSection = document.querySelector("#results-section");
  const resultsList = document.querySelector("#results-list");
  const resultsTitle = document.querySelector("#results-title");
  const resultCount = document.querySelector("#result-count");

  try {
    await loadLibraryItems();
  } catch {
    savedVideoIds.clear();
  }

  form.addEventListener("submit", async (event) => {
    event.preventDefault();
    const query = input.value.trim();
    if (!query) {
      setMessage(status, "Digite o nome de uma música, artista ou vídeo.", "error");
      input.focus();
      return;
    }

    const submitButton = form.querySelector("button[type='submit']");
    submitButton.disabled = true;
    submitButton.dataset.originalText = submitButton.innerHTML;
    submitButton.textContent = "Buscando...";
    resultsSection.hidden = false;
    resultsList.replaceChildren();
    resultsTitle.textContent = `Resultados para “${query}”`;
    setMessage(status, "Buscando vídeos no YouTube...");

    try {
      const videos = await readResponse(await fetch(
        `/api/music-search?q=${encodeURIComponent(query)}&maxResults=12`));
      resultCount.textContent = `${videos.length} ${videos.length === 1 ? "resultado" : "resultados"}`;
      if (videos.length === 0) {
        setMessage(status, "Nenhum vídeo encontrado. Tente outra busca.");
      } else {
        setMessage(status, "");
        resultsList.replaceChildren(...videos.map(makeResultCard));
      }
    } catch (error) {
      setMessage(status, error.message, "error");
    } finally {
      submitButton.disabled = false;
      submitButton.innerHTML = submitButton.dataset.originalText;
      delete submitButton.dataset.originalText;
    }
  });

  const initialQuery = new URLSearchParams(window.location.search).get("q");
  if (initialQuery) {
    input.value = initialQuery;
    form.requestSubmit();
  }
}

function channelFor(music) {
  return (music.nomeCanal || music.nomeArtista || "").trim();
}

function renderHome(items, playlists) {
  document.querySelector("#home-song-count").textContent = items.length;
  document.querySelector("#home-playlist-count").textContent = playlists.length;

  const channels = new Map();
  for (const item of items) {
    const music = item.musica;
    const name = channelFor(music);
    if (!name) continue;
    const key = name.toLocaleLowerCase("pt-BR");
    const entry = channels.get(key) ?? { name, count: 0, image: music.urlCapa };
    entry.count++;
    entry.image ??= music.urlCapa;
    channels.set(key, entry);
  }

  const list = document.querySelector("#home-artists-list");
  const empty = document.querySelector("#home-artists-empty");
  const status = document.querySelector("#home-artists-status");
  const entries = [...channels.values()].sort((left, right) => left.name.localeCompare(right.name));
  document.querySelector("#home-channel-count").textContent = entries.length;
  list.replaceChildren();

  for (const channel of entries) {
    const card = document.createElement("a");
    card.className = "artist-card";
    card.href = `#biblioteca/canal/${encodeURIComponent(channel.name)}`;
    const portrait = document.createElement("span");
    portrait.className = "artist-portrait";
    if (channel.image && /^https:\/\//i.test(channel.image)) {
      const image = document.createElement("img");
      image.src = channel.image;
      image.alt = "";
      image.loading = "lazy";
      image.referrerPolicy = "no-referrer";
      image.addEventListener("error", () => portrait.replaceChildren(makeFallback()), { once: true });
      portrait.append(image);
    } else {
      portrait.append(makeFallback());
    }
    const name = document.createElement("strong");
    name.textContent = channel.name;
    const count = document.createElement("span");
    count.textContent = `${channel.count} ${channel.count === 1 ? "música salva" : "músicas salvas"}`;
    card.append(portrait, name, count);
    list.append(card);
  }

  empty.hidden = entries.length > 0;
  list.hidden = entries.length === 0;
  setMessage(status, "");
}

async function renderHomePage() {
  const status = document.querySelector("#home-artists-status");
  setMessage(status, "Carregando seu espaço...");
  try {
    const [items, playlists] = await Promise.all([
      loadLibraryItems(),
      readResponse(await fetch("/api/playlists"))
    ]);
    renderHome(items, playlists);
  } catch (error) {
    document.querySelector("#home-song-count").textContent = "—";
    document.querySelector("#home-channel-count").textContent = "—";
    document.querySelector("#home-playlist-count").textContent = "—";
    setMessage(status, error.message, "error");
  }
}

function playLibraryMusic(music) {
  if (music.videoExternoId) {
    createYouTubePlayer(
      music.videoExternoId,
      music.titulo,
      channelFor(music) || "Canal não informado",
      music.urlCapa);
    return;
  }

  if (!music.urlAudio) {
    setMessage(document.querySelector("#library-status"), "Esta música não tem um arquivo de áudio configurado.", "error");
    return;
  }
  if (youtubePlayer) {
    youtubePlayer.destroy();
    youtubePlayer = null;
  }
  pendingYouTubeVideo = null;
  hasActivePlayback = true;
  playerPanel.classList.remove("idle");
  playerFrame.replaceChildren();
  const audio = document.createElement("audio");
  audio.controls = true;
  audio.autoplay = true;
  audio.src = music.urlAudio;
  playerFrame.append(audio);
  playerPanel.classList.add("open");
  playerPanel.classList.remove("docked", "large");
  playerBackdrop.classList.add("visible");
  playerTitle.textContent = music.titulo;
  playerTrackTitle.textContent = music.titulo;
  playerTrackChannel.textContent = music.nomeArtista || "Catálogo SoundSpace";
}

function makeLibraryRow(item) {
  const music = item.musica;
  const row = document.createElement("article");
  row.className = "library-row";
  const thumbnail = makeThumbnail(music.urlCapa, music.titulo, true);
  const copy = document.createElement("div");
  copy.className = "library-row-copy";
  const title = document.createElement("strong");
  title.textContent = music.titulo;
  const channel = document.createElement("span");
  channel.textContent = channelFor(music) || "Música do catálogo";
  copy.append(title, channel);

  const playButton = document.createElement("button");
  playButton.className = "icon-action play-library-button";
  playButton.type = "button";
  playButton.setAttribute("aria-label", `Reproduzir ${music.titulo}`);
  playButton.title = "Reproduzir";
  playButton.textContent = "▶";
  playButton.addEventListener("click", () => playLibraryMusic(music));

  const removeButton = document.createElement("button");
  removeButton.className = "icon-action remove-library-button";
  removeButton.type = "button";
  removeButton.setAttribute("aria-label", `Remover ${music.titulo} da biblioteca`);
  removeButton.title = "Remover da biblioteca";
  removeButton.textContent = "×";
  removeButton.addEventListener("click", () => removeFromLibrary(item, removeButton));

  row.append(thumbnail, copy, playButton, removeButton);
  if (music.videoExternoId) row.dataset.videoId = music.videoExternoId;
  return row;
}

async function removeFromLibrary(item, button) {
  const music = item.musica;
  if (!window.confirm(`Remover “${music.titulo}” da biblioteca? A música não será excluída do catálogo.`)) {
    return;
  }

  button.disabled = true;
  const status = document.querySelector("#library-status");
  try {
    await readResponse(await fetch(`/api/biblioteca/${item.musicaId}`, { method: "DELETE" }));
    libraryItems = libraryItems.filter(saved => saved.musicaId !== item.musicaId);
    if (music.videoExternoId) savedVideoIds.delete(music.videoExternoId);
    renderLibrary(libraryItems, readRoute().channel);
  } catch (error) {
    button.disabled = false;
    setMessage(status, error.message, "error");
  }
}

function renderLibrary(items, channelFilter) {
  const list = document.querySelector("#library-list");
  const empty = document.querySelector("#library-empty");
  const status = document.querySelector("#library-status");
  const filterLabel = document.querySelector("#library-filter-label");
  const clearFilter = document.querySelector("#clear-library-filter");
  const title = document.querySelector("#library-empty-title");
  const description = document.querySelector("#library-empty-description");
  const normalizedFilter = channelFilter?.toLocaleLowerCase("pt-BR");
  const filtered = normalizedFilter
    ? items.filter(item => channelFor(item.musica).toLocaleLowerCase("pt-BR") === normalizedFilter)
    : items;

  list.replaceChildren(...filtered.map(makeLibraryRow));
  list.hidden = filtered.length === 0;
  empty.hidden = filtered.length > 0;
  document.querySelector("#library-count").textContent =
    `${filtered.length} ${filtered.length === 1 ? "música" : "músicas"}`;
  filterLabel.hidden = !channelFilter;
  clearFilter.hidden = !channelFilter;
  filterLabel.textContent = channelFilter ? `Mostrando músicas de ${channelFilter}` : "";
  title.textContent = channelFilter ? `Nenhuma música salva de ${channelFilter}` : "Sua biblioteca está vazia";
  description.textContent = channelFilter
    ? "Salve vídeos deste canal para encontrá-los nesta coleção."
    : "Pesquise um vídeo, salve na biblioteca e encontre-o por aqui depois.";
  setMessage(status, "");
}

async function renderLibraryPage(channelFilter) {
  const status = document.querySelector("#library-status");
  setMessage(status, "Carregando sua biblioteca...");
  document.querySelector("#refresh-library").addEventListener("click", () => renderLibraryPage(readRoute().channel));
  document.querySelector("#clear-library-filter").addEventListener("click", () => {
    window.location.hash = "#biblioteca";
  });
  try {
    const items = await loadLibraryItems();
    renderLibrary(items, channelFilter);
  } catch (error) {
    document.querySelector("#library-list").hidden = true;
    document.querySelector("#library-empty").hidden = true;
    setMessage(status, error.message, "error");
  }
}

async function renderRoute() {
  const route = readRoute();
  const view = route.page === "buscar"
    ? "/buscar.html"
    : route.page === "biblioteca"
      ? "/biblioteca.html"
      : "/inicio.html";

  try {
    const response = await fetch(view, { cache: "no-cache" });
    if (!response.ok) throw new Error("Não foi possível abrir esta tela.");
    pageContent.innerHTML = await response.text();
  } catch (error) {
    pageContent.textContent = error.message;
    return;
  }

  document.querySelectorAll(".nav-link[data-route]").forEach(link => {
    const isActive = link.dataset.route === route.page;
    link.classList.toggle("active", isActive);
    if (isActive) link.setAttribute("aria-current", "page");
    else link.removeAttribute("aria-current");
  });
  document.title = route.page === "buscar"
    ? "Buscar — SoundSpace"
    : route.page === "biblioteca"
      ? "Sua biblioteca — SoundSpace"
      : "SoundSpace — Sua música, seu espaço";

  if (route.page === "buscar") await renderSearchPage();
  if (route.page === "biblioteca") await renderLibraryPage(route.channel);
  if (route.page === "inicio") await renderHomePage();
}

document.querySelector("#minimize-player").addEventListener("click", minimizePlayer);
document.querySelector("#expand-player").addEventListener("click", expandPlayer);
document.querySelector("#stop-player").addEventListener("click", stopPlayer);
playerBackdrop.addEventListener("click", minimizePlayer);
document.querySelectorAll("[data-player-size]").forEach(button => {
  button.addEventListener("click", () => {
    playerPanel.classList.toggle("large", button.dataset.playerSize === "large");
    playerPanel.classList.add("open");
    playerPanel.classList.remove("docked");
  });
});
window.addEventListener("hashchange", renderRoute);
renderRoute();
