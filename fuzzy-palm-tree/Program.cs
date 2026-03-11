using System;

public class Channel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Video> Videos { get; set; } = new();
}

public class Video
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ChannelId { get; set; }
    public Channel Channel { get; set; } = null!;
}

public class Track
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public List<Playlist> Playlists { get; set; } = new();
}

public class Playlist
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<Track> Tracks { get; set; } = new();
}

public class YoutubeContext : DbContext
{
    public DbSet<Channel> Channels => Set<Channel>();
    public DbSet<Video> Viideos => Set<Video>();
    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<Playlist> Playlists => Set<Playlist>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=youtube.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Channel>()
            .HasMany(c => c.Videos)
            .WithOne(v => v.Channel)
            .HasForeignKey(v => v.ChannelId);

        modelBuilder.Entity<Playlist>()
            .HasMany(p => p.Tracks)
            .WithMany(t => t.playlists);

        modelBuilder.Entity<Channel>().HasData(new Channel { Id = 1, Name = "Official Music TV" });
        modelBuilder.Entity<Track>().HasData(new Track { Id = 1, Title = "Starboy", Artist = "The Weeknd" });
    }
}

using var db = new YoutubeContext();
db.Database.EnsureDeleted();
db.Database.EnsureCreated();

var channel = new Channel { Name = "Tech Reviews" };
var video = new Video { Title = "iPhone 15 Review" };
channel.Videos.Add(video);
db.Channels.Add(channel);

var playlist = new Playlist { Title = "Chill Hits" };
var track = db.Tracks.First();
playlist.Tracks.Add(track);
db.Playlists.Add(playlist);
db.SaveChanges();

var allChannels = db.Channels.Include(c => c.Videos).ToList();
var myPlaylist = db.Playlists.Include(p => p.Tracks).FirstOrDefault(p => p.Title == "Chill Hits");

var channelToUpdate = db.Channels.First(c => c.Name == "Tech Reviews");
channelToUpdate.Name = "Tech Reviews Updated";
db.SaveChanges();

var trackToDelete = db.Tracks.FirstOrDefault(t => t.Id == 1);
if (trackToDelete != null)
{
    db.Tracks.Remove(trackToDelete);
    db.SaveChanges();
}