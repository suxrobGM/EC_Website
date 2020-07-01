﻿using System.Linq;
using System.Threading.Tasks;
using EC_Website.Core.Entities.Forum;
using EC_Website.Core.Entities.User;
using EC_Website.Core.Interfaces;
using EC_Website.Infrastructure.Data;

namespace EC_Website.Infrastructure.Repositories
{
    public class ForumRepository : Repository, IForumRepository
    {
        private readonly ApplicationDbContext _context;

        public ForumRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddFavoriteThreadAsync(Thread favoriteThread, ApplicationUser user)
        {
            await _context.FavoriteThreads.AddAsync(new FavoriteThread()
            {
                Thread = favoriteThread,
                User = user
            });
            await _context.SaveChangesAsync();
        }

        public async Task DeleteForumAsync(ForumHead forum)
        {
            var sourceForum = _context.ForumHeads.FirstOrDefault(i => i.Id == forum.Id);

            if (sourceForum == null)
                return;

            foreach (var board in sourceForum.Boards)
            {
                await DeleteBoardAsync(board, false);
            }

            _context.Remove(forum);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBoardAsync(Board board, bool saveChanges = true)
        {
            var sourceBoard = _context.Boards.FirstOrDefault(i => i.Id == board.Id);

            if (sourceBoard == null)
                return;

            foreach (var thread in sourceBoard.Threads)
            {
                await DeleteThreadAsync(thread, false);
            }

            _context.Remove(sourceBoard);

            if (saveChanges)
                await _context.SaveChangesAsync();
        }

        public async Task DeleteThreadAsync(Thread thread, bool saveChanges = true)
        {
            var sourceThread = _context.Threads.FirstOrDefault(i => i.Id == thread.Id);

            if (sourceThread == null) return;

            foreach (var post in sourceThread.Posts)
            {
                _context.Posts.Remove(post);
            }

            _context.Remove(sourceThread);

            if (saveChanges)
                await _context.SaveChangesAsync();
        }

        public Task DeleteFavoriteThreadAsync(Thread favoriteThread)
        {
            var sourceFavoriteThread = _context.FavoriteThreads.FirstOrDefault(i => i.ThreadId == favoriteThread.Id);

            if (sourceFavoriteThread == null)
                return Task.CompletedTask;

            _context.Remove(sourceFavoriteThread);
            return _context.SaveChangesAsync();
        }

        public Task DeletePostAsync(Post post)
        {
            var sourcePost = _context.Posts.FirstOrDefault(i => i.Id == post.Id);

            if (sourcePost == null)
                return Task.CompletedTask;

            _context.Remove(sourcePost);
            return _context.SaveChangesAsync();
        }
    }
}