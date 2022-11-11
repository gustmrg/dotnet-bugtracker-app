using BugTracker.Models;

namespace BugTracker.Services.Interfaces;

public interface IProjectService
{
    public Task AddNewProjectAsync(Project project);

    public Task<bool> AddProjectManagerAsync(string userId, int projectId);

    public Task<bool> AddUserToProjectAsync(string userId, int projectId);

    public Task ArchiveProjectAsync(Project project);

    public Task RestoreProjectAsync(Project project);

    public Task<List<Project>> GetAllProjectsByCompany(int companyId);

    public Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName);

    public Task<List<ApplicationUser>> GetAllProjectMembersExceptPMAsync(int projectId);

    public Task<List<Project>> GetArchivedProjectsByCompany(int companyId);

    public Task<List<ApplicationUser>> GetDevelopersOnProjectAsync(int projectId);

    public Task<ApplicationUser> GetProjectManagerAsync(int projectId);

    public Task<List<ApplicationUser>> GetProjectMembersByRoleAsync(int projectId, string role);

    public Task<Project> GetProjectByIdAsync(int projectId, int companyId);

    public Task<List<ApplicationUser>> GetSubmittersOnProjectAsync(int projectId);

    public Task<List<ApplicationUser>> GetUsersNotOnProjectAsync(int projectId, int companyId);

    public Task<List<Project>> GetUserProjectsAsync(string userId);

    public Task<bool> IsUserOnProjectAsync(string userId, int projectId);

    public Task<int> LookupProjectPriorityId(string priorityName);

    public Task RemoveProjectManagerAsync(int projectId);

    public Task RemoveUsersFromProjectByRoleAsync(string role, int projectId);

    public Task RemoveUserFromProjectAsync(string userId, int projectId);

    public Task UpdateProjectAsync(Project project);
}